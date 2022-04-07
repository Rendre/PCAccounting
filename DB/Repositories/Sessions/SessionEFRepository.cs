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

        var item = _db.Session.FirstOrDefault(p =>  p != null && p.ID == id && p.IsDeleted == false);
        return item;
    }

    public List<Session?> GetItems(string? token, DateTime? time, uint userID, string? userIP, uint skip = 0,
        uint take = 1)
    {
        var items = _db.Session.Where(p => p != null && p.IsDeleted == false);

        if (!string.IsNullOrEmpty(token))
        {
            items = items.Where(p => p != null && p.Token != null && p.Token.Equals(token));
        }

        if (time != null)
        {
            items = items.Where(p => p != null && p.Time.Equals(time));
        }

        if (userID > 0)
        {
            items = items.Where(p => p != null && p.UserID == userID);
        }

        if (!string.IsNullOrEmpty(userIP))
        {
            items = items.Where(p => p != null && p.UserIP != null && p.UserIP.Equals(userIP));
        }

        items = items.Skip((int)skip).Take((int)take);

        return items.ToList();
    }

    public int GetItemsCount(string? token, DateTime? time, uint userID, string? userIP)
    {
        var items = _db.Session.Where(p => p != null && p.IsDeleted == false);

        if (!string.IsNullOrEmpty(token))
        {
            items = items.Where(p => p != null && p.Token != null && p.Token.Equals(token));
        }

        if (time != null)
        {
            items = items.Where(p => p != null && p.Time.Equals(time));
        }

        if (userID > 0)
        {
            items = items.Where(p => p != null && p.UserID == userID);
        }

        if (!string.IsNullOrEmpty(userIP))
        {
            items = items.Where(p => p != null && p.UserIP != null && p.UserIP.Equals(userIP));
        }

        return items.Count();
    }

    private bool CreateItem(Session? session)
    {
        _db.Session.Add(session);
        var countOfChanges =  _db.SaveChanges();
        return countOfChanges > 0;
    }

    private bool UpdateItem(Session? session)
    {
        _db.Session.Update(session);
        var countOfChanges = _db.SaveChanges();
        return countOfChanges > 0;
    }
}