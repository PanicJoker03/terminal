using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class BookRepository(TerminalDbContext context) : IBookRepository
{
    public Book Create(string name, string status)
    {
        var book = new Book { Name = name, Status = status };
        context.Books.Add(book);
        context.SaveChanges();
        return book;
    }

    public IReadOnlyList<Book> List() => context.Books.AsNoTracking().OrderBy(book => book.Id).ToList();

    public bool Update(int id, string name, string status)
    {
        Book? book = context.Books.SingleOrDefault(item => item.Id == id);
        if (book is null) return false;
        book.Name = name;
        book.Status = status;
        context.SaveChanges();
        return true;
    }

    public bool Archive(int id) => UpdateStatus(id, "ARCHIVED");

    private bool UpdateStatus(int id, string status)
    {
        Book? book = context.Books.SingleOrDefault(item => item.Id == id);
        if (book is null) return false;
        book.Status = status;
        context.SaveChanges();
        return true;
    }
}
