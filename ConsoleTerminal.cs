namespace GolemSharp
{
    //TODO: document
    public class ConsoleTerminal
    {
        private static ConsoleTerminal? instance;
        public ConsoleColor TextColor = ConsoleColor.White;

        // Hooks for web integration. If set, these will be used instead of Console APIs.
        public static Func<string, Task>? OutputHandler;
        public static Func<Task<string?>>? ReadLineAsyncHandler;
        public static Func<Task>? WaitUserInputAsyncHandler;
        public static Func<Task>? ClearAsyncHandler;

        private ConsoleTerminal() { }

        public static ConsoleTerminal GetInstance()
        {
            if (instance == null)
                instance = new ConsoleTerminal();
            return instance;
        }

        public void PrintLine(object line)
        {
            var text = $"> {line}";
            if (OutputHandler != null)
            {
                OutputHandler(text).GetAwaiter().GetResult();
            }
            else
            {
                Console.ForegroundColor = TextColor;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        public void WaitUserInput()
        {
            if (WaitUserInputAsyncHandler != null)
            {
                WaitUserInputAsyncHandler().GetAwaiter().GetResult();
            }
            else
            {
                Console.ReadKey();
            }
        }

        public void ClearConsole()
        {
            if (ClearAsyncHandler != null)
            {
                ClearAsyncHandler().GetAwaiter().GetResult();
            }
            else
            {
                Console.Clear();
            }
        }
        
        public string? ReadLine()
        {
            if (ReadLineAsyncHandler != null)
            {
                return ReadLineAsyncHandler().GetAwaiter().GetResult();
            }
            else
            {
                return Console.ReadLine();
            }
        }
    }
}