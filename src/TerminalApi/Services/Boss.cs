namespace TerminalApi.Services
{
    public class Boss : IBoss
    {        
        private ConsoleTerminal terminal;
        public Boss()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void CheckBoss()
        {
            terminal.ClearConsole();
            terminal.PrintLine("boss check");
            terminal.PrintLine("how's the boss?");
            string bossStatus = (terminal.ReadLine() ?? "").ToUpper();

            terminal.PrintLine($"boss status: {bossStatus}");
            terminal.WaitUserInput();
        }
    }
}