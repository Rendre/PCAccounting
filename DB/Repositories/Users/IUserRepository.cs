using DB.Entities;

namespace DB.Repositories.Users;

public interface IUserRepository : IDisposable
{
    public bool SaveItem(User? item);

    public User? GetItem(uint id);

    public List<User> GetItems(string? login = null, string? email = null, uint employerID = 0,
                                EntityStatus isActivated = EntityStatus.None, string? activationCode = null, uint skip = 0, uint take = 0);

    public int GetItemsCount(string? login = null, string? email = null, uint employerID = 0, EntityStatus isActivated = EntityStatus.None,
        string? activationCode = null);
}