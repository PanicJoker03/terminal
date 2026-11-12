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
    string Origin,
    string Modality,
    string Priority,
    string? Contact,
    string? RhName,
    string? Channel);

/// <summary>HTTP endpoints for terminal-related data outside the SignalR hub.</summary>
public sealed record PriorityTaskUpdate(bool Completed);

[ApiController]
[Route("api/[controller]")]
public sealed class TerminalController(IJobApplicationRepository jobApplications, IPriorityTasksStore priorityTasks) : ControllerBase
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
                application.Origin,
                application.Modality,
                application.Priority,
                application.Contact,
                application.RhName,
                application.Channel))
            .ToArray();

        return Ok(applications);
    }

    /// <summary>Loads daily tasks from Markdown, converting a legacy text file when needed.</summary>
    [HttpGet("tasks")]
    [ProducesResponseType<IReadOnlyList<PriorityTask>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<PriorityTask>> GetTasks([FromQuery] string? date = null)
    {
        try
        {
            return Ok(priorityTasks.Load(date ?? DateTime.Now.ToString("ddMMyyyy")));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    /// <summary>Updates a task checkbox and persists it to the day's Markdown file.</summary>
    [HttpPut("tasks/{date}/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult SetTaskCompleted(string date, int id, [FromBody] PriorityTaskUpdate update)
    {
        try
        {
            priorityTasks.SetCompleted(date, id, update.Completed);
            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
