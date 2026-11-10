using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public interface IInterviewRepository
{
    Interview Create(
        string attendants,
        string organizer,
        int month,
        int day,
        string hour,
        string status,
        string companyClient,
        string role);

    IReadOnlyList<Interview> List();

    bool Update(Interview interview);
}
