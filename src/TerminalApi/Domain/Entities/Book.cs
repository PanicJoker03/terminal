namespace TerminalApi.Domain.Entities;

public class Book
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
