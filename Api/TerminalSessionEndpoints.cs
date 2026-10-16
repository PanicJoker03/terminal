using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TerminalSharp.Api;

/// <summary>Payload for creating a terminal session.</summary>
public sealed record CreateTerminalSessionRequest
{
    /// <summary>Optional display name used by the existing terminal flow.</summary>
    [MaxLength(100)]
    public string? Name { get; init; }
}

/// <summary>Terminal session information returned by the API.</summary>
public sealed record TerminalSessionResponse(
    string Id,
    string Name,
    bool IsRunning,
    string WebSocketEndpoint);

/// <summary>A buffered terminal event emitted by a session.</summary>
public sealed record TerminalEventResponse(long Sequence, string Type, string? Text);

/// <summary>Terminal events newer than the requested sequence.</summary>
public sealed record TerminalEventsResponse(IReadOnlyList<TerminalEventResponse> Events);

/// <summary>Text entered into a terminal session.</summary>
public sealed record SendTerminalInputRequest
{
    /// <summary>The line of input to send. Empty strings are accepted as Enter.</summary>
    [Required, MaxLength(4096)]
    public required string Text { get; init; }
}

/// <summary>REST endpoints for terminal sessions.</summary>
public static class TerminalSessionEndpoints
{
    public static WebApplication MapTerminalSessionEndpoints(this WebApplication app)
    {
        var sessions = app.MapGroup("/api/terminal-sessions").WithTags("Terminal sessions");

        sessions.MapPost("/", CreateSession)
            .WithName("CreateTerminalSession")
            .WithSummary("Create and start a terminal session")
            .WithDescription("Starts an isolated run of the existing interactive terminal workflow.")
            .Produces<TerminalSessionResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        sessions.MapGet("/{sessionId}", GetSession)
            .WithName("GetTerminalSession")
            .WithSummary("Get terminal session status")
            .Produces<TerminalSessionResponse>()
            .Produces(StatusCodes.Status404NotFound);

        sessions.MapGet("/{sessionId}/events", GetEvents)
            .WithName("GetTerminalSessionEvents")
            .WithSummary("Poll buffered terminal events")
            .WithDescription("Returns output and clear events after the supplied sequence number. SignalR is preferred for live updates.")
            .Produces<TerminalEventsResponse>()
            .Produces(StatusCodes.Status404NotFound);

        sessions.MapPost("/{sessionId}/input", SendInput)
            .WithName("SendTerminalInput")
            .WithSummary("Send a line of input to a terminal session")
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();

        sessions.MapDelete("/{sessionId}", DeleteSession)
            .WithName("DeleteTerminalSession")
            .WithSummary("Stop and delete a terminal session")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static Created<TerminalSessionResponse> CreateSession(
        CreateTerminalSessionRequest request,
        ITerminalSessionService runner,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TerminalSession session = runner.CreateSession(request.Name);
        var response = ToResponse(session);
        return TypedResults.Created($"/api/terminal-sessions/{session.Id}", response);
    }

    private static Results<Ok<TerminalSessionResponse>, NotFound> GetSession(
        string sessionId,
        ITerminalSessionService runner,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return runner.TryGetSession(sessionId, out var session) && session is not null
            ? TypedResults.Ok(ToResponse(session))
            : TypedResults.NotFound();
    }

    private static Results<Ok<TerminalEventsResponse>, NotFound> GetEvents(
        string sessionId,
        ITerminalSessionService runner,
        CancellationToken cancellationToken,
        long after = 0)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!runner.TryGetSession(sessionId, out var session) || session is null)
        {
            return TypedResults.NotFound();
        }

        IReadOnlyList<TerminalEventResponse> events = session.GetEventsAfter(after)
            .Select(item => new TerminalEventResponse(item.Sequence, item.Type, item.Text))
            .ToArray();
        return TypedResults.Ok(new TerminalEventsResponse(events));
    }

    private static Results<Accepted, NotFound, Conflict<string>> SendInput(
        string sessionId,
        SendTerminalInputRequest request,
        ITerminalSessionService runner,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!runner.TryGetSession(sessionId, out var session) || session is null)
        {
            return TypedResults.NotFound();
        }

        return session.EnqueueInput(request.Text)
            ? TypedResults.Accepted($"/api/terminal-sessions/{session.Id}")
            : TypedResults.Conflict("The terminal session is no longer accepting input.");
    }

    private static Results<NoContent, NotFound> DeleteSession(
        string sessionId,
        ITerminalSessionService runner,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return runner.RemoveSession(sessionId)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    private static TerminalSessionResponse ToResponse(TerminalSession session)
    {
        return new TerminalSessionResponse(
            session.Id,
            session.Name,
            session.IsRunning,
            "/ws/terminal");
    }
}
