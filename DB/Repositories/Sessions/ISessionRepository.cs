using DB.Entities;

namespace DB.Repositories.Sessions;

public interface ISessionRepository
{
    public bool CreateItem(Session session);

    public bool UpdateItem(Session session);

    public Session? GetItem(string token);

    // public bool DeleteItem();

}