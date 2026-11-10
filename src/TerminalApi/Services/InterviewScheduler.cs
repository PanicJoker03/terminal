using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class InterviewScheduler : IInterviewScheduler
{
    private readonly ConsoleTerminal terminal;
    private readonly IInterviewRepository repository;

    public InterviewScheduler(IInterviewRepository repository)
    {
        terminal = ConsoleTerminal.GetInstance();
        this.repository = repository;
    }

    public void ManageInterviews()
    {
        while (true)
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("    interview scheduler    ");
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("enter action (schedule / load / update / exit):");

            string action = JobApplicationTracker.NormalizeAction(terminal.ReadLine());
            switch (action)
            {
                case "schedule":
                    ScheduleInterview();
                    break;
                case "load":
                    LoadInterviews();
                    break;
                case "update":
                    UpdateInterview();
                    break;
                case "exit":
                    terminal.PrintLine("interview scheduler finalized.");
                    return;
                default:
                    terminal.PrintLine("invalid action. use schedule, load, update or exit.");
                    terminal.PrintLine("press any key to continue...");
                    terminal.WaitUserInput();
                    break;
            }
        }
    }

    private void ScheduleInterview()
    {
        terminal.ClearConsole();
        terminal.PrintLine("----------------------------");
        terminal.PrintLine("     schedule interview    ");
        terminal.PrintLine("----------------------------");

        string attendants = ReadRequired("attendants:");
        string organizer = ReadRequired("organizer:");
        int month = ReadNumber("month (1-12):", 1, 12);
        int day = ReadNumber("day (1-31):", 1, 31);
        string hour = ReadRequired("hour (for example, 2:30 PM):");
        string status = ReadOptional("status (default: scheduled):");
        if (string.IsNullOrWhiteSpace(status))
        {
            status = "SCHEDULED";
        }
        string companyClient = ReadRequired("company/client:");
        string role = ReadRequired("role:");

        try
        {
            repository.Create(attendants, organizer, month, day, hour, status, companyClient, role);
            terminal.PrintLine("interview scheduled successfully.");
        }
        catch (Exception exception)
        {
            terminal.PrintLine($"failed to schedule interview: {exception.Message}");
        }

        ReturnPrompt();
    }

    private void LoadInterviews()
    {
        terminal.ClearConsole();
        terminal.PrintLine("----------------------------");
        terminal.PrintLine("     scheduled interviews  ");
        terminal.PrintLine("----------------------------");

        try
        {
            IReadOnlyList<Interview> interviews = repository.List();
            if (interviews.Count == 0)
            {
                terminal.PrintLine("no interviews found.");
            }
            else
            {
                PrintInterviews(interviews);
            }
        }
        catch (Exception exception)
        {
            terminal.PrintLine($"failed to load interviews: {exception.Message}");
        }

        ReturnPrompt();
    }

    private void UpdateInterview()
    {
        terminal.ClearConsole();
        terminal.PrintLine("----------------------------");
        terminal.PrintLine("     update interview      ");
        terminal.PrintLine("----------------------------");

        IReadOnlyList<Interview> interviews;
        try
        {
            interviews = repository.List();
        }
        catch (Exception exception)
        {
            terminal.PrintLine($"failed to load interviews: {exception.Message}");
            ReturnPrompt();
            return;
        }

        if (interviews.Count == 0)
        {
            terminal.PrintLine("no interviews found.");
            ReturnPrompt();
            return;
        }

        PrintInterviews(interviews);
        terminal.PrintLine("select an interview by number:");
        string? selection = terminal.ReadLine();
        if (!int.TryParse(selection, out int selectedNumber) || selectedNumber < 1 || selectedNumber > interviews.Count)
        {
            terminal.PrintLine("invalid interview number.");
            ReturnPrompt();
            return;
        }

        Interview selected = interviews[selectedNumber - 1];
        terminal.PrintLine("press enter to keep a current value.");
        string attendants = ReadOptional($"attendants [{selected.Attendants}]:");
        string organizer = ReadOptional($"organizer [{selected.Organizer}]:");
        int month = ReadOptionalNumber($"month [{selected.Month}]:", selected.Month, 1, 12);
        int day = ReadOptionalNumber($"day [{selected.Day}]:", selected.Day, 1, 31);
        string hour = ReadOptional($"hour [{selected.Hour}]:");
        string status = ReadOptional($"status [{selected.Status}]:");
        string companyClient = ReadOptional($"company/client [{selected.CompanyClient}]:");
        string role = ReadOptional($"role [{selected.Role}]:");

        var updatedInterview = new Interview
        {
            Id = selected.Id,
            Attendants = ValueOrExisting(attendants, selected.Attendants),
            Organizer = ValueOrExisting(organizer, selected.Organizer),
            Month = month,
            Day = day,
            Hour = ValueOrExisting(hour, selected.Hour),
            Status = ValueOrExisting(status, selected.Status),
            CompanyClient = ValueOrExisting(companyClient, selected.CompanyClient),
            Role = ValueOrExisting(role, selected.Role)
        };

        try
        {
            terminal.PrintLine(repository.Update(updatedInterview)
                ? "interview updated successfully."
                : "interview was not found.");
        }
        catch (Exception exception)
        {
            terminal.PrintLine($"failed to update interview: {exception.Message}");
        }

        ReturnPrompt();
    }

    private void PrintInterviews(IReadOnlyList<Interview> interviews)
    {
        for (int index = 0; index < interviews.Count; index++)
        {
            Interview interview = interviews[index];
            terminal.PrintLine(
                $"{index + 1}. {interview.Month:D2}/{interview.Day:D2} {interview.Hour} | " +
                $"{interview.CompanyClient} - {interview.Role} | {interview.Status} | " +
                $"attendants: {interview.Attendants} | organizer: {interview.Organizer}");
        }
    }

    private string ReadRequired(string prompt)
    {
        while (true)
        {
            terminal.PrintLine(prompt);
            string value = ReadOptional("");
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            terminal.PrintLine("value is required.");
        }
    }

    private string ReadOptional(string prompt)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            terminal.PrintLine(prompt);
        }

        return (terminal.ReadLine() ?? "").Trim();
    }

    private int ReadNumber(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            terminal.PrintLine(prompt);
            if (int.TryParse(terminal.ReadLine(), out int value) && value >= minimum && value <= maximum)
            {
                return value;
            }

            terminal.PrintLine($"enter a number from {minimum} to {maximum}.");
        }
    }

    private int ReadOptionalNumber(string prompt, int currentValue, int minimum, int maximum)
    {
        while (true)
        {
            string value = ReadOptional(prompt);
            if (string.IsNullOrWhiteSpace(value))
            {
                return currentValue;
            }

            if (int.TryParse(value, out int number) && number >= minimum && number <= maximum)
            {
                return number;
            }

            terminal.PrintLine($"enter a number from {minimum} to {maximum}.");
        }
    }

    private static string ValueOrExisting(string value, string existingValue)
    {
        return string.IsNullOrWhiteSpace(value) ? existingValue : value;
    }

    private void ReturnPrompt()
    {
        terminal.PrintLine("press any key to return...");
        terminal.WaitUserInput();
    }
}
