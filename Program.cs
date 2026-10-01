using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TerminalSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<TerminalRunner>();

var app = builder.Build();

app.UsePathBase("/terminal");

app.UseWebSockets();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<TerminalHub>("/terminalHub");

app.MapGet("/start", (TerminalRunner runner) =>
{
    runner.Start();
    return Results.Ok();
});

app.Run();