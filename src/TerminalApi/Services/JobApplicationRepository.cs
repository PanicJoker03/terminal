using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class JobApplicationRepository(TerminalDbContext context) : IJobApplicationRepository
{
    public JobApplication Create(
        DateTime timestamp,
        string company,
        string role,
        string status,
        string payment,
        string origin,
        string modality = "on-site",
        string priority = "C")
    {
        var application = new JobApplication
        {
            Timestamp = timestamp,
            Company = company,
            Role = role,
            Status = status,
            Payment = payment,
            Origin = origin,
            Modality = string.IsNullOrWhiteSpace(modality) ? "on-site" : modality,
            Priority = string.IsNullOrWhiteSpace(priority) ? "C" : priority
        };

        context.JobApplications.Add(application);
        context.SaveChanges();
        return application;
    }

    public IReadOnlyList<JobApplication> List()
    {
        return context.JobApplications
            .AsNoTracking()
            .OrderBy(application => application.Timestamp)
            .ThenBy(application => application.Id)
            .ToList();
    }

    public bool UpdateStatus(int id, string status)
    {
        JobApplication? application = context.JobApplications.SingleOrDefault(item => item.Id == id);
        if (application is null)
        {
            return false;
        }

        application.Status = status;
        context.SaveChanges();
        return true;
    }
}
