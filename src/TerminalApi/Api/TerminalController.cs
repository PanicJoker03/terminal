using Microsoft.AspNetCore.Mvc;
using TerminalApi.Services;

namespace TerminalApi.Api;

/// <summary>A persisted job application returned by the API.</summary>
public sealed record JobApplicationResponse(
    int Id,
    DateTime Timestamp,
    string Company,
    string Role,
    string Status,
    string Payment,
    string Origin);

/// <summary>HTTP endpoints for terminal-related data outside the SignalR hub.</summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TerminalController(IJobApplicationRepository jobApplications) : ControllerBase
{
    /// <summary>Returns persisted job applications from SQLite.</summary>
    [HttpGet("jobs-applications")]
    [ProducesResponseType<IReadOnlyList<JobApplicationResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<JobApplicationResponse>> GetJobApplications()
    {
        IReadOnlyList<JobApplicationResponse> applications = jobApplications.List()
            .Select(application => new JobApplicationResponse(
                application.Id,
                application.Timestamp,
                application.Company,
                application.Role,
                application.Status,
                application.Payment,
                application.Origin))
            .ToArray();

        return Ok(applications);
    }
}