using Xunit;

namespace TerminalSharp.Tests;

public class ConsoleTerminalTests
{
    [Fact]
    public void PrintLine_Formats_Output_In_Lowercase()
    {
        string? output = null;
        using var io = ConsoleTerminal.UseWebIo(
            text =>
            {
                output = text;
                return Task.CompletedTask;
            },
            () => Task.FromResult<string?>(null),
            () => Task.CompletedTask,
            () => Task.CompletedTask);

        ConsoleTerminal.GetInstance().PrintLine("HELLO MAYAN CONSULTING");

        Assert.Equal("> hello mayan consulting", output);
    }
}