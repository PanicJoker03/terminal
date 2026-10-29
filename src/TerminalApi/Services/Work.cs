namespace TerminalApi.Services
{
    public class Work : IWork
    {        
        private ConsoleTerminal terminal;
        public Work()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        
        public void CheckWork()
        {
            terminal.ClearConsole();
            terminal.PrintLine("work check");
            terminal.PrintLine("how's the work?");
            string WorkStatus = (terminal.ReadLine() ?? "").ToUpper();

            terminal.PrintLine($"work status: {WorkStatus}");
            terminal.WaitUserInput();
        }
    }
}