namespace TerminalApi.Services;

public sealed record PriorityTask(int Id, string Text, bool Completed);

public interface IPriorityTasksStore
{
    IReadOnlyList<PriorityTask> Load(string date);
    void SetCompleted(string date, int id, bool completed);
}
