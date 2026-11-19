namespace TerminalApi.Services;

public sealed record PriorityTask(int Id, string Text, bool Completed);
public sealed record DatedPriorityTask(string Date, PriorityTask Task);

public interface IPriorityTasksStore
{
    IReadOnlyList<PriorityTask> Load(string date);
    IReadOnlyList<DatedPriorityTask> LoadIncompleteTasks();
    void SetCompleted(string date, int id, bool completed);
    void Save(string date, IReadOnlyList<PriorityTask> tasks);
    IReadOnlyList<string> ListDates();
    int ImportLegacyFiles();
}
