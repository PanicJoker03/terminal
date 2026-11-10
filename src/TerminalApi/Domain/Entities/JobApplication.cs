namespace TerminalApi.Domain.Entities;

public class JobApplication
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Company { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Payment { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Modality { get; set; } = "on-site";

    public string Priority { get; set; } = "C";
}
