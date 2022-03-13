namespace DB.Repositories.User;
using Entities;

public interface IUserRepository : IDisposable
{
    public void CreateItem(User user);

    public bool ChangeItem(uint id, string? login, string? password, uint employerID);

    public User? GetItem(uint id);

    public List<User> GetItems();

    public User? GetItem(string? login);

    public uint DeleteItem(uint id);
}