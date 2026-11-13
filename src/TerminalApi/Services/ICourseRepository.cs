using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public interface ICourseRepository
{
    Course Create(string name, int progress, string status);
    IReadOnlyList<Course> List();
    bool Update(int id, string name, int progress, string status);
    bool Archive(int id);
}
