using TerminalApi;
using TerminalApi.Api;
using TerminalApi.Data;
using TerminalApi.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Configure Jwt:Key with a random secret of at least 32 characters.");
string authUsername = builder.Configuration["Auth:Username"] ?? throw new InvalidOperationException("Configure Auth:Username.");
string authPassword = builder.Configuration["Auth:Password"] ?? throw new InvalidOperationException("Configure Auth:Password.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32) throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");
const string jwtIssuer = "terminal-api";
const string jwtAudience = "terminal-front";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            string? accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/ws/terminal"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = jwtIssuer,
        ValidateAudience = true, ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true, IssuerSigningKey = signingKey,
        ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

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
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IPriorityTasksStore, PriorityTasksStore>();
builder.Services.AddTransient<IPrioritiesChecker, PrioritiesChecker>();
builder.Services.AddTransient<IJobApplicationTracker, JobApplicationTracker>();
builder.Services.AddTransient<IInterviewScheduler, InterviewScheduler>();
builder.Services.AddTransient<ICourseTracker, CourseTracker>();
builder.Services.AddTransient<IBookTracker, BookTracker>();
builder.Services.AddTransient<IClientTracker, ClientTracker>();
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
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string assemblyHash = Convert.ToHexString(
    System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(typeof(Program).Assembly.Location))).ToLowerInvariant();
DateTimeOffset startedAt = DateTimeOffset.UtcNow;
app.MapGet("/api/version", () => Results.Ok(new { sha256 = assemblyHash, startedAt })).AllowAnonymous();
app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (!System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(request.Username ?? ""), Encoding.UTF8.GetBytes(authUsername))
        || !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(request.Password ?? ""), Encoding.UTF8.GetBytes(authPassword)))
        return Results.Unauthorized();

    DateTime expires = DateTime.UtcNow.AddHours(8);
    var claims = new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, authUsername) };
    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(jwtIssuer, jwtAudience, claims,
        notBefore: DateTime.UtcNow, expires: expires,
        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
    return Results.Ok(new { accessToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token), expiresAt = expires });
}).AllowAnonymous();

app.MapHub<TerminalHub>("/ws/terminal").RequireAuthorization();
app.MapControllers();
app.MapTerminalSessionEndpoints();

app.Run();

public sealed record LoginRequest(string? Username, string? Password);
