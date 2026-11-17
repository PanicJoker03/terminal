namespace TerminalApi.Services
{
    public class RealityCheck : IRealityCheck
    {        
        private ConsoleTerminal terminal;
        public RealityCheck()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        
        public void CoinToss()
        {
            terminal.ClearConsole();
            terminal.PrintLine("reality check");
            terminal.PrintLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            terminal.PrintLine("press any key to finalize module...");
            terminal.WaitUserInput();
        }
    }
}
