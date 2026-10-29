namespace TerminalApi.Services;

public interface IJobApplicationRepository
{
    JobApplication Create(DateTime timestamp, string company, string role, string status, string payment, string origin);

    IReadOnlyList<JobApplication> List();

    bool UpdateStatus(int id, string status);
}