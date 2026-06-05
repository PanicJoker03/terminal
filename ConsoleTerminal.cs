namespace GolemSharp
{
    public class ConsoleTerminal
    {
        private static ConsoleTerminal? instance;

        public ConsoleColor TextColor = ConsoleColor.White;

        private ConsoleTerminal(){}

        public static ConsoleTerminal GetInstance()
        {
            if(instance == null)
                instance = new ConsoleTerminal();
            return instance;
        }

        public void PrintLine(object line)
        {
            Console.ForegroundColor = TextColor;
            Console.WriteLine($"> {line}");
            Console.ResetColor();
        }
        
        public void WaitUserInput()
        {
            Console.ReadKey();
        }
        
        public void ClearConsole()
        {
            Console.Clear();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}