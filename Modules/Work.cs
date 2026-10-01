namespace TerminalSharp.Modules
{
    public class Work
    {        
        private ConsoleTerminal terminal;
        public Work()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        
        public void CheckWork()
        {
            terminal.ClearConsole();
            terminal.PrintLine("WORK CHECK");
            terminal.PrintLine("HOW'S THE WORK?");
            string WorkStatus = (terminal.ReadLine() ?? "").ToUpper();

            terminal.PrintLine($"WORK STATUS: {WorkStatus}");
            terminal.WaitUserInput();
        }
    }
}