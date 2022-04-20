using DB.Entities;

namespace DB.Repositories.Users;

public interface IUserRepository
{
    public bool SaveItem(User? item);

    public User? GetItem(uint id);

    public List<User> GetItems(string? login = null, string? email = null, bool search = false, uint employerID = 0,
                                EntityStatus isActivated = EntityStatus.None, string? activationCode = null, uint skip = 0, uint take = 0);

    public uint GetItemsCount(string? login = null, string? email = null, bool search = false, uint employerID = 0, EntityStatus isActivated = EntityStatus.None,
        string? activationCode = null);
}