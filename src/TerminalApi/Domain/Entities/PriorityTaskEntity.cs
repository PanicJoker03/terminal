namespace TerminalApi.Domain.Entities;

public class PriorityTaskEntity
{
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public int Position { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Completed { get; set; }
}
