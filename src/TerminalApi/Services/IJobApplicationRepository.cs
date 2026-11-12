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
        string priority = "C",
        string? contact = null,
        string? rhName = null,
        string? channel = null);

    IReadOnlyList<JobApplication> List();

    bool UpdateStatus(int id, string status);
}
