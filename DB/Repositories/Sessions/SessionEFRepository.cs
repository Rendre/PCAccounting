using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories.Sessions;

public class SessionEFRepository : ISessionRepository
{
    private readonly ApplicationContextEF _db;
    public SessionEFRepository()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }

    public bool SaveItem(Session? item)
    {
        if (item == null) return false;

        return item.ID switch
        {
            0 => CreateItem(item),
            > 0 => UpdateItem(item)
        };
    }

    public Session? GetItem(uint id)
    {
        if (id == 0) return null;

        return _db.Session.FirstOrDefault(p =>  p != null && p.ID == id && p.IsDeleted == false);
    }

    public List<Session?> GetItems(string? token, uint userID, string? userIP, uint skip = 0, uint take = 1)
    {
        var sessions = _db.Session.Where(p => p != null && p.IsDeleted == false);

        if (!string.IsNullOrEmpty(token))
        {
            sessions = sessions.Where(p => p != null && p.Token != null && p.Token.Equals(token));
        }

        if (userID > 0)
        {
            sessions = sessions.Where(p => p != null && p.UserID == userID);
        }

        if (!string.IsNullOrEmpty(userIP))
        {
            sessions = sessions.Where(p => p != null && p.UserIP != null && p.UserIP.Equals(userIP));
        }

        sessions = sessions.Skip((int)skip).Take((int)take);

        return sessions.ToList();
    }

    public int GetItemsCount(string? token, uint userID, string? userIP)
    {
        var sessions = _db.Session.Where(p => p != null && p.IsDeleted == false);

        if (!string.IsNullOrEmpty(token))
        {
            sessions = sessions.Where(p => p != null && p.Token != null && p.Token.Equals(token));
        }

        if (userID > 0)
        {
            sessions = sessions.Where(p => p != null && p.UserID == userID);
        }

        if (!string.IsNullOrEmpty(userIP))
        {
            sessions = sessions.Where(p => p != null && p.UserIP != null && p.UserIP.Equals(userIP));
        }

        return sessions.Count();
    }

    private bool CreateItem(Session? session)
    {
        _db.Session.Add(session);
        var stateCount =  _db.SaveChanges();
        return stateCount > 0;
    }

    private bool UpdateItem(Session? session)
    {
        _db.Session.Update(session);
        var stateCount = _db.SaveChanges();
        return stateCount > 0;
    }
}