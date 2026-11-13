namespace TerminalApi.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Projects { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
