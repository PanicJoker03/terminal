using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class CourseTracker(ICourseRepository repository) : ICourseTracker
{
    private readonly ConsoleTerminal terminal = ConsoleTerminal.GetInstance();

    public void ManageCourses()
    {
        while (true)
        {
            terminal.ClearConsole();
            terminal.PrintLine("courses: add / load / modify / archive / exit");
            string action = JobApplicationTracker.NormalizeAction(terminal.ReadLine());
            switch (action)
            {
                case "add": AddCourse(); break;
                case "load": ListCourses(); Pause(); break;
                case "modify": ModifyCourse(); break;
                case "archive": ArchiveCourse(); break;
                case "exit":
                case "quit": return;
                default: terminal.PrintLine("invalid action."); Pause(); break;
            }
        }
    }

    private void AddCourse()
    {
        terminal.PrintLine("course name:");
        string name = ReadRequired();
        terminal.PrintLine("progress (0-100):");
        int progress = ReadProgress();
        terminal.PrintLine("status:");
        string status = ReadRequired();
        repository.Create(name, progress, status);
        terminal.PrintLine("course saved.");
        Pause();
    }

    private void ListCourses()
    {
        IReadOnlyList<Course> courses = repository.List();
        if (courses.Count == 0) terminal.PrintLine("no courses found.");
        foreach (Course course in courses)
            terminal.PrintLine($"{course.Id}. {course.Name} | {course.Progress}% | {course.Status}");
    }

    private void ModifyCourse()
    {
        IReadOnlyList<Course> courses = repository.List();
        if (!TrySelect(courses, out Course? selected)) return;
        terminal.PrintLine($"name (current: {selected!.Name}):");
        string name = ReadOptional(selected.Name);
        terminal.PrintLine($"progress 0-100 (current: {selected.Progress}):");
        int progress = ReadProgress(selected.Progress);
        terminal.PrintLine($"status (current: {selected.Status}):");
        string status = ReadOptional(selected.Status);
        repository.Update(selected.Id, name, progress, status);
        terminal.PrintLine("course updated.");
        Pause();
    }

    private void ArchiveCourse()
    {
        IReadOnlyList<Course> courses = repository.List();
        if (!TrySelect(courses, out Course? selected)) return;
        repository.Archive(selected!.Id);
        terminal.PrintLine("course archived.");
        Pause();
    }

    private bool TrySelect(IReadOnlyList<Course> courses, out Course? selected)
    {
        selected = null;
        if (courses.Count == 0) { terminal.PrintLine("no courses found."); Pause(); return false; }
        for (int index = 0; index < courses.Count; index++)
        {
            Course course = courses[index];
            terminal.PrintLine($"{index + 1}. {course.Name} | {course.Progress}% | {course.Status}");
        }
        terminal.PrintLine("number:");
        if (!int.TryParse(terminal.ReadLine(), out int number) || number < 1 || number > courses.Count)
        { terminal.PrintLine("invalid selection."); Pause(); return false; }
        selected = courses[number - 1];
        return true;
    }

    private string ReadRequired() => (terminal.ReadLine() ?? string.Empty).Trim();
    private string ReadOptional(string current)
    {
        string value = (terminal.ReadLine() ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(value) ? current : value;
    }

    private int ReadProgress(int? current = null)
    {
        while (true)
        {
            string? input = terminal.ReadLine();
            if (current.HasValue && string.IsNullOrWhiteSpace(input)) return current.Value;
            if (int.TryParse(input, out int progress) && progress is >= 0 and <= 100) return progress;
            terminal.PrintLine("enter a whole number from 0 to 100:");
        }
    }

    private void Pause() { terminal.PrintLine("press any key to return..."); terminal.WaitUserInput(); }
}
