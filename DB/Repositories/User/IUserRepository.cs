namespace DB.Repositories.User;
using Entities;

public interface IUserRepository : IDisposable
{
    public User? GetItem(uint id);

    public List<User> GetItems();

    public User? GetItem(string login);

    public uint DeleteItem(uint id);

    public uint CreateUser(string login, string password, uint employerId);

    public uint ChangeUser(uint id, string login, string password, uint employerID);

}