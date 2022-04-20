using DB.Entities;

namespace DB.Repositories.Sessions;

public interface ISessionRepository
{
    public bool SaveItem(Session? item);

    public Session? GetItem(uint id);

    public List<Session?> GetItems(string? token = null, DateTime? time = null, uint userID = 0,
        string? userIP = null, uint skip = 0, uint take = 0);

    public uint GetItemsCount(string? token = null, DateTime? time = null, uint userID = 0,
        string? userIP = null);
}