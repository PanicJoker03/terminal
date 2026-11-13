using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class ClientTracker(IClientRepository repository) : IClientTracker
{
    private readonly ConsoleTerminal terminal = ConsoleTerminal.GetInstance();

    public void ManageClients()
    {
        while (true)
        {
            terminal.ClearConsole();
            terminal.PrintLine("clients: add / load / update / delete / exit");
            string action = JobApplicationTracker.NormalizeAction(terminal.ReadLine());
            switch (action)
            {
                case "add": AddClient(); break;
                case "load": ListClients(); Pause(); break;
                case "update": UpdateClient(); break;
                case "delete": DeleteClient(); break;
                case "exit":
                case "quit": return;
                default: terminal.PrintLine("invalid action."); Pause(); break;
            }
        }
    }

    private void AddClient()
    {
        terminal.PrintLine("client name:");
        string name = ReadRequired();
        terminal.PrintLine("project(s):");
        string projects = ReadRequired();
        terminal.PrintLine("status:");
        string status = ReadRequired();
        repository.Create(name, projects, status);
        terminal.PrintLine("client saved.");
        Pause();
    }

    private void ListClients()
    {
        IReadOnlyList<Client> clients = repository.List();
        if (clients.Count == 0) terminal.PrintLine("no clients found.");
        foreach (Client client in clients)
            terminal.PrintLine($"{client.Id}. {client.Name} | projects: {client.Projects} | {client.Status}");
    }

    private void UpdateClient()
    {
        if (!TrySelect(out Client? selected)) return;
        terminal.PrintLine($"name (current: {selected!.Name}):");
        string name = ReadOptional(selected.Name);
        terminal.PrintLine($"project(s) (current: {selected.Projects}):");
        string projects = ReadOptional(selected.Projects);
        terminal.PrintLine($"status (current: {selected.Status}):");
        string status = ReadOptional(selected.Status);
        repository.Update(selected.Id, name, projects, status);
        terminal.PrintLine("client updated.");
        Pause();
    }

    private void DeleteClient()
    {
        if (!TrySelect(out Client? selected)) return;
        repository.Delete(selected!.Id);
        terminal.PrintLine("client deleted.");
        Pause();
    }

    private bool TrySelect(out Client? selected)
    {
        IReadOnlyList<Client> clients = repository.List();
        selected = null;
        if (clients.Count == 0) { terminal.PrintLine("no clients found."); Pause(); return false; }
        for (int index = 0; index < clients.Count; index++)
        {
            Client client = clients[index];
            terminal.PrintLine($"{index + 1}. {client.Name} | projects: {client.Projects} | {client.Status}");
        }
        terminal.PrintLine("number:");
        if (!int.TryParse(terminal.ReadLine(), out int number) || number < 1 || number > clients.Count)
        { terminal.PrintLine("invalid selection."); Pause(); return false; }
        selected = clients[number - 1];
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
