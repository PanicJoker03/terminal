using System.Text.RegularExpressions;

namespace TerminalApi.Services;

public sealed class PriorityTasksStore(IConfiguration configuration) : IPriorityTasksStore
{
    private readonly string tasksDirectory = Path.GetFullPath(
        configuration["FilePaths:Tasks"] ?? "tasks", AppContext.BaseDirectory);
    private readonly string resultsDirectory = Path.GetFullPath(
        configuration["FilePaths:Results"] ?? "results", AppContext.BaseDirectory);

    public IReadOnlyList<PriorityTask> Load(string date)
    {
        ValidateDate(date);
        Directory.CreateDirectory(tasksDirectory);
        string markdownPath = MarkdownPath(date);
        if (!File.Exists(markdownPath))
        {
            string legacyPath = LegacyPath(date);
            if (!File.Exists(legacyPath))
            {
                return [];
            }

            string markdown = string.Join(Environment.NewLine, File.ReadAllLines(legacyPath)
                .Select(ParseLegacyLine)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => $"- [ ] {line}"));
            File.WriteAllText(markdownPath, markdown + (markdown.Length > 0 ? Environment.NewLine : ""));
            File.Delete(legacyPath);
        }

        return ReadMarkdown(markdownPath);
    }

    public IReadOnlyList<DatedPriorityTask> LoadIncompleteTasks()
    {
        if (!Directory.Exists(tasksDirectory))
        {
            return [];
        }

        return Directory.EnumerateFiles(tasksDirectory, "OUT_*.md")
            .Select(path => new
            {
                Path = path,
                Date = Path.GetFileNameWithoutExtension(path).Replace("OUT_", "", StringComparison.Ordinal)
            })
            .Where(file => file.Date.Length == 8 && DateTime.TryParseExact(
                file.Date,
                "ddMMyyyy",
                null,
                System.Globalization.DateTimeStyles.None,
                out _))
            .OrderBy(file => DateTime.ParseExact(
                file.Date,
                "ddMMyyyy",
                null,
                System.Globalization.DateTimeStyles.None))
            .SelectMany(file => ReadMarkdown(file.Path)
                .Where(task => !task.Completed)
                .Select(task => new DatedPriorityTask(file.Date, task)))
            .ToArray();
    }

    public void SetCompleted(string date, int id, bool completed)
    {
        ValidateDate(date);
        if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

        string path = MarkdownPath(date);
        if (!File.Exists(path))
        {
            _ = Load(date);
        }
        if (!File.Exists(path)) throw new KeyNotFoundException("Task file was not found.");

        var tasks = ReadMarkdown(path).ToList();
        if (id >= tasks.Count) throw new KeyNotFoundException("Task was not found.");
        tasks[id] = tasks[id] with { Completed = completed };
        File.WriteAllLines(path, tasks.Select(task => $"- [{(task.Completed ? 'x' : ' ')}] {task.Text}"));
    }

    private static IReadOnlyList<PriorityTask> ReadMarkdown(string path) => File.ReadAllLines(path)
        .Select(line => Regex.Match(line, @"^\s*[-*+]\s+\[([ xX])\]\s+(.*)$"))
        .Where(match => match.Success)
        .Select((match, id) => new PriorityTask(id, match.Groups[2].Value, match.Groups[1].Value != " "))
        .ToArray();

    private static string ParseLegacyLine(string line)
    {
        var priority = Regex.Match(line, @"^\d{8}\s+PRIORITY\s+[A-Z]\s+IS:\s*(.*)$", RegexOptions.IgnoreCase);
        if (priority.Success) return priority.Groups[1].Value.Trim();
        var extra = Regex.Match(line, @"^another task is:\s*(.*)$", RegexOptions.IgnoreCase);
        return extra.Success ? extra.Groups[1].Value.Trim() : line.Trim();
    }

    private string MarkdownPath(string date) => Path.Combine(tasksDirectory, $"OUT_{date}.md");
    private string LegacyPath(string date) => Path.Combine(resultsDirectory, $"OUT_{date}.txt");

    private static void ValidateDate(string date)
    {
        if (date.Length != 8 || !DateTime.TryParseExact(date, "ddMMyyyy", null,
                System.Globalization.DateTimeStyles.None, out _))
            throw new ArgumentException("Date must use ddMMyyyy format.", nameof(date));
    }
}
