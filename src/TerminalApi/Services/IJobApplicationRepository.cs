using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public interface IJobApplicationRepository
{
    JobApplication Create(
        DateTime timestamp,
        string company,
        string role,
        string status,
        string payment,
        string origin,
        string modality = "on-site",
        string priority = "C");

    IReadOnlyList<JobApplication> List();

    bool UpdateStatus(int id, string status);
}
