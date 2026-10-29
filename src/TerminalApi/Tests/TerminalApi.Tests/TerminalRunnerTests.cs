using Xunit;

namespace TerminalApi.Tests;

public class TerminalRunnerTests
{
    [Theory]
    [InlineData("<enter>", "")]
    [InlineData(" <ENTER> ", "")]
    [InlineData("hello", "hello")]
    [InlineData(null, "")]
    public void NormalizeInput_Maps_Enter_Token_To_Empty_Input(string? input, string expected)
    {
        Assert.Equal(expected, TerminalSession.NormalizeInput(input));
    }
}