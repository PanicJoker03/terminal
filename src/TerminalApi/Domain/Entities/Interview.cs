namespace TerminalApi.Domain.Entities;

public class Interview
{
    public int Id { get; set; }

    public string Attendants { get; set; } = string.Empty;

    public string Organizer { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Day { get; set; }

    public string Hour { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string CompanyClient { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
