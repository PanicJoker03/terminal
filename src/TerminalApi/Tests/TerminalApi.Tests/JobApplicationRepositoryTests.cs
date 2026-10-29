using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;
using TerminalApi.Services;
using Xunit;

namespace TerminalApi.Tests;

public class JobApplicationRepositoryTests
{
    [Fact]
    public void Create_List_And_UpdateStatus_Persist_Expected_Values()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TerminalDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var setupContext = new TerminalDbContext(options))
        {
            setupContext.Database.EnsureCreated();
        }

        int firstId;
        using (var createContext = new TerminalDbContext(options))
        {
            var repository = new JobApplicationRepository(createContext);
            JobApplication first = repository.Create(
                new DateTime(2026, 10, 29, 10, 0, 0),
                "Acme, Inc.",
                "Engineer",
                "SENT",
                "120000",
                "LinkedIn");
            repository.Create(
                new DateTime(2026, 10, 30, 10, 0, 0),
                "Beta",
                "Developer",
                "SENT",
                "",
                "");
            firstId = first.Id;
        }

        using var verifyContext = new TerminalDbContext(options);
        var verifyRepository = new JobApplicationRepository(verifyContext);
        IReadOnlyList<JobApplication> applications = verifyRepository.List();

        Assert.Equal(2, applications.Count);
        Assert.Equal("Acme, Inc.", applications[0].Company);
        Assert.True(verifyRepository.UpdateStatus(firstId, "INTERVIEW"));
        Assert.Equal("INTERVIEW", verifyRepository.List()[0].Status);
        Assert.Equal("Beta", verifyRepository.List()[1].Company);
    }
}