using Microsoft.AspNetCore.SignalR;

namespace TerminalSharp
{
    public class TerminalHub : Hub
    {
        private readonly TerminalRunner _runner;

        public TerminalHub(TerminalRunner runner)
        {
            _runner = runner;
        }

        public async Task<bool> JoinSession(string sessionId)
        {
            if (!_runner.TryGetSession(sessionId, out var session) || session is null)
            {
                return false;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, session.Id);
            return true;
        }

        public Task<bool> SendInput(string sessionId, string input)
        {
            return Task.FromResult(
                _runner.TryGetSession(sessionId, out var session)
                && session is not null
                && session.EnqueueInput(input));
        }
    }
}
