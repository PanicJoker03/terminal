using Microsoft.Extensions.Configuration;
using TerminalApi.Services;
using Xunit;

namespace TerminalApi.Tests;

public class PriorityCheckerTests
{
    [Fact]
    public void Relative_results_path_is_resolved_from_application_directory()
    {
        string directoryName = $"priority-results-{Guid.NewGuid():N}";
        string expectedDirectory = Path.Combine(AppContext.BaseDirectory, directoryName);

        try
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["FilePaths:Results"] = directoryName
                })
                .Build();

            using var io = ConsoleTerminal.UseWebIo(
                _ => Task.CompletedTask,
                () => Task.FromResult<string?>("no"),
                () => Task.CompletedTask,
                () => Task.CompletedTask);

            new PrioritiesChecker(configuration).TakePriorities();

            string outputPath = Path.Combine(
                expectedDirectory,
                $"OUT_{DateTime.Now:ddMMyyyy}.txt");

            Assert.True(File.Exists(outputPath));
            Assert.Contains("PRIORITY A IS: NO", File.ReadAllText(outputPath));
        }
        finally
        {
            if (Directory.Exists(expectedDirectory))
            {
                Directory.Delete(expectedDirectory, recursive: true);
            }

        }
    }
}