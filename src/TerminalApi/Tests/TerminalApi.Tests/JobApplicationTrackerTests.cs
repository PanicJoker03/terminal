using TerminalApi.Modules;
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

    [Fact]
    public void UpdateStatusLine_Updates_The_Status_Field_Only()
    {
        string[] lines =
        [
            "TIMESTAMP,COMPANY,ROLE,STATUS,PAYMENT,ORIGIN",
            "2026-10-14 10:00:00,Acme,Engineer,SENT,120000,LinkedIn"
        ];

        var updated = JobApplicationTracker.UpdateStatusLine(lines, 1, "INTERVIEW");

        Assert.Equal("2026-10-14 10:00:00,Acme,Engineer,INTERVIEW,120000,LinkedIn", updated[1]);
        Assert.Equal("TIMESTAMP,COMPANY,ROLE,STATUS,PAYMENT,ORIGIN", updated[0]);
    }
}
