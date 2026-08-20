using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GolemSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<GolemRunner>();

var app = builder.Build();

app.UsePathBase("/terminal");

app.UseWebSockets();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<TerminalHub>("/terminalHub");

app.MapGet("/start", (GolemRunner runner) =>
{
    runner.Start();
    return Results.Ok();
});

app.Run();