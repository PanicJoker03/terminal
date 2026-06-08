using Microsoft.AspNetCore.SignalR;

namespace GolemSharp
{
    public class TerminalHub : Hub
    {
        private readonly GolemRunner _runner;

        public TerminalHub(GolemRunner runner)
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
