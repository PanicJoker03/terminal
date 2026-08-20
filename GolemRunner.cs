using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;

namespace GolemSharp
{
    public class GolemRunner
    {
        //TODO: Document
        private readonly IHubContext<TerminalHub> _hub;
        private readonly Channel<string> _input = Channel.CreateUnbounded<string>();
        private Task? _runningTask;
        private readonly object _lock = new();

        public GolemRunner(IHubContext<TerminalHub> hub)
        {
            _hub = hub;

            ConsoleTerminal.OutputHandler = async (text) =>
            {
                await _hub.Clients.All.SendAsync("output", text);
            };

            ConsoleTerminal.ClearAsyncHandler = async () =>
            {
                await _hub.Clients.All.SendAsync("clear");
            };

            ConsoleTerminal.ReadLineAsyncHandler = async () =>
            {
                var value = await _input.Reader.ReadAsync();
                return value;
            };

            ConsoleTerminal.WaitUserInputAsyncHandler = async () =>
            {
                await _input.Reader.ReadAsync();
            };
        }

        public void EnqueueInput(string input)
        {
            // mirror ENTER behavior for ReadLine: if empty, send empty string
            _input.Writer.TryWrite(input ?? string.Empty);
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_runningTask != null && !_runningTask.IsCompleted)
                    return;

                _runningTask = Task.Run(() =>
                {
                    var golem = new GolemSharp("MAYAN CONSULTING", ConsoleColor.Yellow);
                    golem.Run();
                });
            }
        }
    }
}
