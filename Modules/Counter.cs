namespace GolemSharp.Modules
{
    public class Counter
    {        
        private ConsoleTerminal terminal;
        public Counter()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void CountToTen()
        {
            terminal.ClearConsole();
            terminal.PrintLine("COUNT TO TEN:");

            for (int counter = 1; counter <= 10; counter++)
            {
                Thread.Sleep(1000);
                terminal.PrintLine(counter);
            }

            terminal.PrintLine("PRESS ANY KEY TO EXIT PROGRAM...");
            terminal.WaitUserInput();
        }
    }
}