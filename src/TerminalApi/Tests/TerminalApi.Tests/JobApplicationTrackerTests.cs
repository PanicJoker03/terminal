using TerminalApi.Services;
using Xunit;

namespace TerminalApi.Tests;

public class JobApplicationTrackerTests
{
    [Theory]
    [InlineData(" APPLY ", "apply")]
    [InlineData("EXIT", "exit")]
    [InlineData(null, "")]
    public void NormalizeAction_Uses_Lowercase_Validation_Values(string? input, string expected)
    {
        Assert.Equal(expected, JobApplicationTracker.NormalizeAction(input));
    }

}
