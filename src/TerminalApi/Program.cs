using TerminalApi;
using TerminalApi.Api;
using TerminalApi.Data;
using TerminalApi.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
string databaseConnectionString = Environment.GetEnvironmentVariable("TERMINAL_SQLITE_PATH") is { } environmentPath
    ? $"Data Source={environmentPath}"
    : builder.Configuration.GetConnectionString("TerminalDatabase")
        ?? "Data Source=/var/lib/terminal-app/terminal.db";
var sqliteConnectionString = new SqliteConnectionStringBuilder(databaseConnectionString);
string? databaseDirectory = Path.GetDirectoryName(sqliteConnectionString.DataSource);
if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

builder.Services.AddDbContext<TerminalDbContext>(options =>
    options.UseSqlite(sqliteConnectionString.ConnectionString));
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddSingleton<IPriorityTasksStore, PriorityTasksStore>();
builder.Services.AddTransient<IPrioritiesChecker, PrioritiesChecker>();
builder.Services.AddTransient<IJobApplicationTracker, JobApplicationTracker>();
builder.Services.AddTransient<IInterviewScheduler, InterviewScheduler>();
builder.Services.AddTransient<IBoss, Boss>();
builder.Services.AddTransient<IWork, Work>();
builder.Services.AddTransient<IRealityCheck, RealityCheck>();
builder.Services.AddTransient<ICounter, Counter>();
builder.Services.AddTransient<IPomodoro, Pomodoro>();
builder.Services.AddCors(options =>
{
    string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:4200"];

    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});
builder.Services.AddSingleton<TerminalRunner>();
builder.Services.AddSingleton<ITerminalSessionService>(services =>
    services.GetRequiredService<TerminalRunner>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    TerminalDbContext dbContext = scope.ServiceProvider.GetRequiredService<TerminalDbContext>();
    dbContext.Database.Migrate();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
});
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string assemblyHash = Convert.ToHexString(
    System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(typeof(Program).Assembly.Location))).ToLowerInvariant();
DateTimeOffset startedAt = DateTimeOffset.UtcNow;
app.MapGet("/api/version", () => Results.Ok(new { sha256 = assemblyHash, startedAt }));

app.MapHub<TerminalHub>("/ws/terminal");
app.MapControllers();
app.MapTerminalSessionEndpoints();

app.Run();
