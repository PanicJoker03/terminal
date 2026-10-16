namespace TerminalSharp
{
    public class ConsoleTerminal
    {
        private static ConsoleTerminal? instance;
        private static readonly AsyncLocal<IoHandlers?> CurrentHandlers = new();
        public ConsoleColor TextColor = ConsoleColor.White;

        private ConsoleTerminal() { }

        public static IDisposable UseWebIo(
            Func<string, Task> output,
            Func<Task<string?>> readLine,
            Func<Task> waitForInput,
            Func<Task> clear)
        {
            var previous = CurrentHandlers.Value;
            CurrentHandlers.Value = new IoHandlers(output, readLine, waitForInput, clear);
            return new IoScope(previous);
        }

        public static ConsoleTerminal GetInstance()
        {
            if (instance == null)
                instance = new ConsoleTerminal();
            return instance;
        }

        public void PrintLine(object line)
        {
            var text = $"> {line}";
            var handlers = CurrentHandlers.Value;
            if (handlers != null)
            {
                handlers.Output(text).GetAwaiter().GetResult();
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
            var handlers = CurrentHandlers.Value;
            if (handlers != null)
            {
                handlers.WaitForInput().GetAwaiter().GetResult();
            }
            else
            {
                Console.ReadKey();
            }
        }

        public void ClearConsole()
        {
            var handlers = CurrentHandlers.Value;
            if (handlers != null)
            {
                handlers.Clear().GetAwaiter().GetResult();
            }
            else
            {
                Console.Clear();
            }
        }
        
        public string? ReadLine()
        {
            var handlers = CurrentHandlers.Value;
            if (handlers != null)
            {
                return handlers.ReadLine().GetAwaiter().GetResult();
            }
            else
            {
                return Console.ReadLine();
            }
        }

        private sealed record IoHandlers(
            Func<string, Task> Output,
            Func<Task<string?>> ReadLine,
            Func<Task> WaitForInput,
            Func<Task> Clear);

        private sealed class IoScope(IoHandlers? previous) : IDisposable
        {
            public void Dispose()
            {
                CurrentHandlers.Value = previous;
            }
        }
    }
}