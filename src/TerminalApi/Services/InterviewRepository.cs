using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class InterviewRepository(TerminalDbContext context) : IInterviewRepository
{
    public Interview Create(
        string attendants,
        string organizer,
        int month,
        int day,
        string hour,
        string status,
        string companyClient,
        string role)
    {
        var interview = new Interview
        {
            Attendants = attendants,
            Organizer = organizer,
            Month = month,
            Day = day,
            Hour = hour,
            Status = status,
            CompanyClient = companyClient,
            Role = role
        };

        context.Interviews.Add(interview);
        context.SaveChanges();
        return interview;
    }

    public IReadOnlyList<Interview> List()
    {
        return context.Interviews
            .AsNoTracking()
            .OrderBy(interview => interview.Month)
            .ThenBy(interview => interview.Day)
            .ThenBy(interview => interview.Hour)
            .ThenBy(interview => interview.Id)
            .ToList();
    }

    public bool Update(Interview interview)
    {
        Interview? existingInterview = context.Interviews.SingleOrDefault(item => item.Id == interview.Id);
        if (existingInterview is null)
        {
            return false;
        }

        existingInterview.Attendants = interview.Attendants;
        existingInterview.Organizer = interview.Organizer;
        existingInterview.Month = interview.Month;
        existingInterview.Day = interview.Day;
        existingInterview.Hour = interview.Hour;
        existingInterview.Status = interview.Status;
        existingInterview.CompanyClient = interview.CompanyClient;
        existingInterview.Role = interview.Role;
        context.SaveChanges();
        return true;
    }
}
