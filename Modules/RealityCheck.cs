namespace GolemSharp.Modules
{
    public class RealityCheck
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
            terminal.PrintLine("REALITY CHECK");
            terminal.PrintLine(reality);

            terminal.PrintLine("PRESS ANY KEY TO FINALIZE MODULE...");
            terminal.WaitUserInput();
        }
    }
}