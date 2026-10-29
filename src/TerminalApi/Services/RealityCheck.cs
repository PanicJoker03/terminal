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
            int reality = new Random().Next(0, 2);

            terminal.ClearConsole();
            terminal.PrintLine("reality check");
            terminal.PrintLine(reality);

            terminal.PrintLine("press any key to finalize module...");
            terminal.WaitUserInput();
        }
    }
}