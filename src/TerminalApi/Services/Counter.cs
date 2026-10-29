namespace TerminalApi.Services
{
    public class Counter : ICounter
    {        
        private ConsoleTerminal terminal;
        public Counter()
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void CountToTen()
        {
            terminal.ClearConsole();
            terminal.PrintLine("count to ten:");

            for (int counter = 1; counter <= 10; counter++)
            {
                Thread.Sleep(1000);
                terminal.PrintLine(counter);
            }

            terminal.PrintLine("press any key to exit module...");
            terminal.WaitUserInput();
        }
    }
}