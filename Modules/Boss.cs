namespace TerminalSharp.Modules
{
    public class Boss
    {        
        private ConsoleTerminal terminal;
        public Boss()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void CheckBoss()
        {
            terminal.ClearConsole();
            terminal.PrintLine("BOSS CHECK");
            terminal.PrintLine("HOW'S THE BOSS?");
            string bossStatus = (terminal.ReadLine() ?? "").ToUpper();

            terminal.PrintLine($"BOSS STATUS: {bossStatus}");
            terminal.WaitUserInput();
        }
    }
}