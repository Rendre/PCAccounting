using DB.Entities;

namespace DB.Repositories.Users;

public interface IUserRepository : IDisposable
{
    public bool CreateItem(User user);

    public bool UpdateItem(User user);

    public User? GetItem(uint id);

    public List<User> GetItems();

    public User? GetItem(string? login);

    public uint DeleteItem(uint id);
}