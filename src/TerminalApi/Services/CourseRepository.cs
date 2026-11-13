using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class CourseRepository(TerminalDbContext context) : ICourseRepository
{
    public Course Create(string name, int progress, string status)
    {
        var course = new Course { Name = name, Progress = progress, Status = status };
        context.Courses.Add(course);
        context.SaveChanges();
        return course;
    }

    public IReadOnlyList<Course> List() => context.Courses.AsNoTracking().OrderBy(course => course.Id).ToList();

    public bool Update(int id, string name, int progress, string status)
    {
        Course? course = context.Courses.SingleOrDefault(item => item.Id == id);
        if (course is null) return false;
        course.Name = name;
        course.Progress = progress;
        course.Status = status;
        context.SaveChanges();
        return true;
    }

    public bool Archive(int id) => UpdateStatus(id, "ARCHIVED");

    private bool UpdateStatus(int id, string status)
    {
        Course? course = context.Courses.SingleOrDefault(item => item.Id == id);
        if (course is null) return false;
        course.Status = status;
        context.SaveChanges();
        return true;
    }
}
