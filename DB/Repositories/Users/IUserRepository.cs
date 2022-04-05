using DB.Entities;

namespace DB.Repositories.Users;

public interface IUserRepository : IDisposable
{
    public bool SaveItem(User? item);

    public User? GetItem(uint id);

    public List<User> GetItems(string? login, string? email, uint employerID,
                                bool isActivated, string? activationCode, uint skip, uint take);

    public int GetItemsCount(string? login, string? email, uint employerID, bool isActivated,
        string? activationCode);
}


//public bool CreateItem(User user);

//public bool UpdateItem(User user);

//public List<User> GetItems();

//public User? GetItem(string? login);

//public User? GetItemByEmail(string? email);

//public bool DeleteItem(uint id);