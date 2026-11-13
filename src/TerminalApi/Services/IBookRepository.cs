using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public interface IBookRepository
{
    Book Create(string name, string status);
    IReadOnlyList<Book> List();
    bool Update(int id, string name, string status);
    bool Archive(int id);
}
