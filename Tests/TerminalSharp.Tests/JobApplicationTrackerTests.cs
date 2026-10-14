using TerminalSharp.Modules;
using Xunit;

namespace TerminalSharp.Tests;

public class JobApplicationTrackerTests
{
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
