using TerminalApi.Domain.Entities;

namespace TerminalApi.Services;

public interface IClientRepository
{
    Client Create(string name, string projects, string status);
    IReadOnlyList<Client> List();
    bool Update(int id, string name, string projects, string status);
    bool Delete(int id);
}
