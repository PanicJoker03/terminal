using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace TerminalApi
{
    public interface ITerminalSessionService
    {
        TerminalSession CreateSession(string? name);
        bool TryGetSession(string id, out TerminalSession? session);
        bool RemoveSession(string id);
    }

    public sealed class TerminalRunner : ITerminalSessionService
    {
        private readonly IHubContext<TerminalHub> _hub;
        private readonly IServiceProvider _services;
        private readonly ConcurrentDictionary<string, TerminalSession> _sessions = new();

        public TerminalRunner(IHubContext<TerminalHub> hub, IServiceProvider services)
        {
            _hub = hub;
            _services = services;
        }

        public TerminalSession CreateSession(string? name)
        {
            string id = Guid.NewGuid().ToString("D");
            var session = new TerminalSession(id, name, _hub, _services, () => RemoveCompletedSessionById(id));
            if (!_sessions.TryAdd(id, session))
            {
                throw new InvalidOperationException("Could not create a terminal session.");
            }

            session.Start();
            return session;
        }

        private void RemoveCompletedSessionById(string id)
        {
            if (_sessions.TryRemove(id, out TerminalSession? session))
            {
                session.Dispose();
            }
        }

        public bool TryGetSession(string id, out TerminalSession? session)
        {
            if (Guid.TryParse(id, out Guid sessionId)
                && _sessions.TryGetValue(sessionId.ToString("D"), out session))
            {
                return true;
            }

            session = null;
            return false;
        }

        public bool RemoveSession(string id)
        {
            if (!TryGetSession(id, out var session) || session is null)
            {
                return false;
            }

            if (!_sessions.TryRemove(session.Id, out _))
            {
                return false;
            }

            session.Dispose();
            return true;
        }
    }

    public sealed record TerminalEvent(long Sequence, string Type, string? Text);

    public sealed class TerminalSession : IDisposable
    {
        private const int MaximumBufferedEvents = 1000;
        private readonly IHubContext<TerminalHub> _hub;
        private readonly IServiceProvider _services;
        private readonly Action _onCompleted;
        private readonly Channel<string> _input = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        private readonly CancellationTokenSource _cancellation = new();
        private readonly CancellationToken _cancellationToken;
        private readonly object _eventLock = new();
        private readonly Queue<TerminalEvent> _events = new();
        private Task? _runningTask;
        private long _nextSequence;
        private int _disposed;

        internal TerminalSession(string id, string? name, IHubContext<TerminalHub> hub, IServiceProvider services, Action onCompleted)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? "JOB ASSISTANT APP" : name.Trim();
            _hub = hub;
            _services = services;
            _onCompleted = onCompleted;
            _cancellationToken = _cancellation.Token;
        }

        public string Id { get; }
        public string Name { get; }
        public bool IsRunning => _runningTask is { IsCompleted: false };

        internal void Start()
        {
            _runningTask = Task.Run(() =>
            {
                using var io = ConsoleTerminal.UseWebIo(
                    text => PublishAsync("output", text),
                    ReadInputAsync,
                    WaitForInputAsync,
                    () => PublishAsync("clear", null));

                try
                {
                    ActivatorUtilities.CreateInstance<TerminalApi>(
                        _services,
                        Name,
                        ConsoleColor.Yellow).Run();
                }
                catch (OperationCanceledException) when (_cancellation.IsCancellationRequested)
                {
                }
                catch (ChannelClosedException) when (_cancellation.IsCancellationRequested)
                {
                }
            });
            _ = _runningTask.ContinueWith(
                _ => _onCompleted(),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public bool EnqueueInput(string? value)
        {
            return IsRunning && _input.Writer.TryWrite(NormalizeInput(value));
        }

        public static string NormalizeInput(string? value)
        {
            return string.Equals(value?.Trim(), "<enter>", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : value ?? string.Empty;
        }

        public IReadOnlyList<TerminalEvent> GetEventsAfter(long sequence)
        {
            lock (_eventLock)
            {
                return _events.Where(item => item.Sequence > sequence).ToArray();
            }
        }

        private async Task<string?> ReadInputAsync()
        {
            return await _input.Reader.ReadAsync(_cancellationToken);
        }

        private async Task WaitForInputAsync()
        {
            await _input.Reader.ReadAsync(_cancellationToken);
        }

        private async Task PublishAsync(string type, string? text)
        {
            TerminalEvent terminalEvent;
            lock (_eventLock)
            {
                terminalEvent = new TerminalEvent(++_nextSequence, type, text);
                _events.Enqueue(terminalEvent);
                if (_events.Count > MaximumBufferedEvents)
                {
                    _events.Dequeue();
                }
            }

            await _hub.Clients.Group(Id).SendAsync("terminalEvent", terminalEvent);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _cancellation.Cancel();
            _input.Writer.TryComplete();
            _cancellation.Dispose();
        }
    }
}
