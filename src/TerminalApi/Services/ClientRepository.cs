using Microsoft.EntityFrameworkCore;
using TerminalApi.Data;
using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public class ClientRepository(TerminalDbContext context) : IClientRepository
{
    public Client Create(string name, string projects, string status)
    {
        var client = new Client { Name = name, Projects = projects, Status = status };
        context.Clients.Add(client);
        context.SaveChanges();
        return client;
    }

    public IReadOnlyList<Client> List() => context.Clients.AsNoTracking().OrderBy(client => client.Id).ToList();

    public bool Update(int id, string name, string projects, string status)
    {
        Client? client = context.Clients.SingleOrDefault(item => item.Id == id);
        if (client is null) return false;
        client.Name = name;
        client.Projects = projects;
        client.Status = status;
        context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        Client? client = context.Clients.SingleOrDefault(item => item.Id == id);
        if (client is null) return false;
        context.Clients.Remove(client);
        context.SaveChanges();
        return true;
    }
}
