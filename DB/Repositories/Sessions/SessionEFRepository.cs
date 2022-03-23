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

    public bool CreateItem(Session session)
    {
        _db.Session.Add(session);
        var stateCount =  _db.SaveChanges();
        return stateCount > 0;
    }

    public bool UpdateItem(Session session)
    {
        _db.Session.Update(session);
        var stateCount = _db.SaveChanges();
        return stateCount > 0;
    }

    public Session? GetItem(string token)
    {
        return _db.Session.FirstOrDefault(p => p != null && p.Token != null && p.Token.Equals(token));
    }
}