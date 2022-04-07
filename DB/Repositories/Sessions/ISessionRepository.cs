using DB.Entities;

namespace DB.Repositories.Sessions;

public interface ISessionRepository
{
    public bool SaveItem(Session? item);

    public Session? GetItem(uint id);

    public List<Session?> GetItems(string? token, DateTime time, uint userID, string? userIP,
        uint skip, uint take);

    public int GetItemsCount(string? token, DateTime time, uint userID, string? userIP);
}

//public bool CreateItem(Session session);

//public bool UpdateItem(Session session);

//public Session? GetItem(string token);

// public bool DeleteItem();