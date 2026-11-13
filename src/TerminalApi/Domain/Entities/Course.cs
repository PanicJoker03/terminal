namespace TerminalApi.Domain.Entities;

public class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Progress { get; set; }

    public string Status { get; set; } = string.Empty;
}
