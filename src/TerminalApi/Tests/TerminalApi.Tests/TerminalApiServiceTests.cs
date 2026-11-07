using TerminalApi.Services;
using Xunit;

namespace TerminalApi.Tests;

public class TerminalApiServiceTests
{
    [Fact]
    public void Service_Implementations_Implement_Their_Interfaces()
    {
        Assert.IsAssignableFrom<IBoss>(new Boss());
        Assert.IsAssignableFrom<ICounter>(new Counter());
        Assert.IsAssignableFrom<IPomodoro>(new Pomodoro());
        Assert.True(typeof(IJobApplicationTracker).IsAssignableFrom(typeof(JobApplicationTracker)));
        Assert.IsAssignableFrom<IPrioritiesChecker>(new PrioritiesChecker());
        Assert.IsAssignableFrom<IRealityCheck>(new RealityCheck());
        Assert.IsAssignableFrom<IWork>(new Work());
    }
}