namespace DB.Repositories.User;
using Entities;

public interface IUserRepository : IDisposable
{
    public User? GetItem(int id);

    public List<User> GetItems();

    public User? GetItem(string login);

    public int DeleteItem(int id);

    public int CreateUser(string login, string password, int employerId);

    public int ChangeUser(int id, string login, string password, int employerID);

}