using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TerminalApi.Services;

public class TerminalDbContextFactory : IDesignTimeDbContextFactory<TerminalDbContext>
{
    public TerminalDbContext CreateDbContext(string[] args)
    {
        string databasePath = Environment.GetEnvironmentVariable("TERMINAL_SQLITE_PATH")
            ?? "terminal.db";

        var optionsBuilder = new DbContextOptionsBuilder<TerminalDbContext>();
        optionsBuilder.UseSqlite($"Data Source={databasePath}");

        return new TerminalDbContext(optionsBuilder.Options);
    }
}