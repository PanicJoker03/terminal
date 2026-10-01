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

        public Task SendInput(string input)
        {
            _runner.EnqueueInput(input);
            return Task.CompletedTask;
        }

        public Task StartSession()
        {
            _runner.Start();
            return Task.CompletedTask;
        }
    }
}
