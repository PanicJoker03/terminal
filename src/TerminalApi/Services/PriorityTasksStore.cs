using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public sealed class PriorityTasksStore(TerminalDbContext context, IConfiguration configuration) : IPriorityTasksStore
{
    private readonly string tasksDirectory = Path.GetFullPath(
        configuration["FilePaths:Tasks"] ?? "tasks", AppContext.BaseDirectory);
    private readonly string resultsDirectory = Path.GetFullPath(
        configuration["FilePaths:Results"] ?? "results", AppContext.BaseDirectory);

    public IReadOnlyList<PriorityTask> Load(string date)
    {
        ValidateDate(date);
        return context.PriorityTasks.AsNoTracking()
            .Where(task => task.Date == date)
            .OrderBy(task => task.Position)
            .Select(task => new PriorityTask(task.Position, task.Text, task.Completed))
            .ToArray();
    }

    public IReadOnlyList<DatedPriorityTask> LoadIncompleteTasks() => context.PriorityTasks
        .AsNoTracking()
        .Where(task => !task.Completed)
        .OrderBy(task => task.Date)
        .ThenBy(task => task.Position)
        .Select(task => new DatedPriorityTask(task.Date,
            new PriorityTask(task.Position, task.Text, task.Completed)))
        .ToArray()
        .OrderBy(task => DateTime.ParseExact(task.Date, "ddMMyyyy", CultureInfo.InvariantCulture))
        .ThenBy(task => task.Task.Id)
        .ToArray();

    public void SetCompleted(string date, int id, bool completed)
    {
        ValidateDate(date);
        if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));
        PriorityTaskEntity? task = context.PriorityTasks.SingleOrDefault(item => item.Date == date && item.Position == id);
        if (task is null) throw new KeyNotFoundException("Task was not found.");
        task.Completed = completed;
        context.SaveChanges();
    }

    public void Save(string date, IReadOnlyList<PriorityTask> tasks)
    {
        ValidateDate(date);
        using var transaction = context.Database.BeginTransaction();
        List<PriorityTaskEntity> existing = context.PriorityTasks.Where(task => task.Date == date).ToList();
        context.PriorityTasks.RemoveRange(existing);
        context.SaveChanges();
        context.PriorityTasks.AddRange(tasks.Select((task, position) => new PriorityTaskEntity
        {
            Date = date,
            Position = position,
            Text = task.Text,
            Completed = task.Completed
        }));
        context.SaveChanges();
        transaction.Commit();
    }

    public IReadOnlyList<string> ListDates() => context.PriorityTasks
        .AsNoTracking()
        .Select(task => task.Date)
        .Distinct()
        .ToArray()
        .OrderByDescending(date => DateTime.ParseExact(date, "ddMMyyyy", CultureInfo.InvariantCulture))
        .ToArray();

    public int ImportLegacyFiles()
    {
        var sources = new List<(string Date, string Path, bool IsMarkdown)>();
        if (Directory.Exists(tasksDirectory))
        {
            sources.AddRange(Directory.EnumerateFiles(tasksDirectory, "OUT_*.md")
                .Select(path => (ExtractDate(path), path, true)));
        }
        if (Directory.Exists(resultsDirectory))
        {
            sources.AddRange(Directory.EnumerateFiles(resultsDirectory, "OUT_*.txt")
                .Select(path => (ExtractDate(path), path, false)));
        }

        int importedFiles = 0;
        foreach (var source in sources.Where(source => IsValidDate(source.Date))
                     .OrderBy(source => source.Date)
                     .ThenByDescending(source => source.IsMarkdown))
        {
            IReadOnlyList<PriorityTask> tasks = source.IsMarkdown
                ? ReadMarkdown(source.Path)
                : ReadLegacyText(source.Path);
            if (tasks.Count == 0)
            {
                continue;
            }

            List<PriorityTaskEntity> alreadyStored = context.PriorityTasks.AsNoTracking()
                .Where(task => task.Date == source.Date)
                .OrderBy(task => task.Position)
                .ToList();
            if (alreadyStored.Count > 0)
            {
                bool sameData = alreadyStored.Count == tasks.Count && alreadyStored
                    .Zip(tasks)
                    .All(pair => pair.First.Position == pair.Second.Id
                        && pair.First.Text == pair.Second.Text
                        && pair.First.Completed == pair.Second.Completed);
                if (sameData)
                {
                    File.Delete(source.Path);
                    importedFiles++;
                }
                continue;
            }

            Save(source.Date, tasks);

            int storedCount = context.PriorityTasks.Count(task => task.Date == source.Date);
            if (storedCount != tasks.Count)
            {
                throw new InvalidOperationException($"Task import verification failed for {source.Path}.");
            }

            File.Delete(source.Path);
            importedFiles++;
        }

        return importedFiles;
    }

    private static IReadOnlyList<PriorityTask> ReadMarkdown(string path) => File.ReadAllLines(path)
        .Select(line => Regex.Match(line, @"^\s*[-*+]\s+\[([ xX])\]\s+(.*)$"))
        .Where(match => match.Success)
        .Select((match, position) => new PriorityTask(position, match.Groups[2].Value, match.Groups[1].Value != " "))
        .ToArray();

    private static IReadOnlyList<PriorityTask> ReadLegacyText(string path) => File.ReadAllLines(path)
        .Select(ParseLegacyLine)
        .Where(text => !string.IsNullOrWhiteSpace(text))
        .Select((text, position) => new PriorityTask(position, text, false))
        .ToArray();

    private static string ParseLegacyLine(string line)
    {
        Match priority = Regex.Match(line, @"^\d{8}\s+PRIORITY\s+[A-Z]\s+IS:\s*(.*)$", RegexOptions.IgnoreCase);
        if (priority.Success) return priority.Groups[1].Value.Trim();
        Match extra = Regex.Match(line, @"^another task is:\s*(.*)$", RegexOptions.IgnoreCase);
        return extra.Success ? extra.Groups[1].Value.Trim() : line.Trim();
    }

    private static string ExtractDate(string path) =>
        Path.GetFileNameWithoutExtension(path).Replace("OUT_", "", StringComparison.Ordinal);

    private static bool IsValidDate(string date) => date.Length == 8 &&
        DateTime.TryParseExact(date, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

    private static void ValidateDate(string date)
    {
        if (!IsValidDate(date))
            throw new ArgumentException("Date must use ddMMyyyy format.", nameof(date));
    }
}
