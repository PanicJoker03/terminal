using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class BookTracker(IBookRepository repository) : IBookTracker
{
    private readonly ConsoleTerminal terminal = ConsoleTerminal.GetInstance();

    public void ManageBooks()
    {
        while (true)
        {
            terminal.ClearConsole();
            terminal.PrintLine("books: add / load / modify / archive / exit");
            string action = JobApplicationTracker.NormalizeAction(terminal.ReadLine());
            switch (action)
            {
                case "add": AddBook(); break;
                case "load": ListBooks(); Pause(); break;
                case "modify": ModifyBook(); break;
                case "archive": ArchiveBook(); break;
                case "exit":
                case "quit": return;
                default: terminal.PrintLine("invalid action."); Pause(); break;
            }
        }
    }

    private void AddBook()
    {
        terminal.PrintLine("book name:");
        string name = ReadRequired();
        terminal.PrintLine("status:");
        string status = ReadRequired();
        repository.Create(name, status);
        terminal.PrintLine("book saved.");
        Pause();
    }

    private void ListBooks()
    {
        IReadOnlyList<Book> books = repository.List();
        if (books.Count == 0) terminal.PrintLine("no books found.");
        foreach (Book book in books) terminal.PrintLine($"{book.Id}. {book.Name} | {book.Status}");
    }

    private void ModifyBook()
    {
        IReadOnlyList<Book> books = repository.List();
        if (!TrySelect(books, out Book? selected)) return;
        terminal.PrintLine($"name (current: {selected!.Name}):");
        string name = ReadOptional(selected.Name);
        terminal.PrintLine($"status (current: {selected.Status}):");
        string status = ReadOptional(selected.Status);
        repository.Update(selected.Id, name, status);
        terminal.PrintLine("book updated.");
        Pause();
    }

    private void ArchiveBook()
    {
        if (!TrySelect(repository.List(), out Book? selected)) return;
        repository.Archive(selected!.Id);
        terminal.PrintLine("book archived.");
        Pause();
    }

    private bool TrySelect(IReadOnlyList<Book> books, out Book? selected)
    {
        selected = null;
        if (books.Count == 0) { terminal.PrintLine("no books found."); Pause(); return false; }
        for (int index = 0; index < books.Count; index++) terminal.PrintLine($"{index + 1}. {books[index].Name} | {books[index].Status}");
        terminal.PrintLine("number:");
        if (!int.TryParse(terminal.ReadLine(), out int number) || number < 1 || number > books.Count)
        { terminal.PrintLine("invalid selection."); Pause(); return false; }
        selected = books[number - 1];
        return true;
    }

    private string ReadRequired() => (terminal.ReadLine() ?? string.Empty).Trim();
    private string ReadOptional(string current)
    {
        string value = (terminal.ReadLine() ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(value) ? current : value;
    }
    private void Pause() { terminal.PrintLine("press any key to return..."); terminal.WaitUserInput(); }
}
