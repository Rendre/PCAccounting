using DB;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Services;

public class LoginService
{
    private readonly ApplicationContextEF _db;

    public LoginService()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }

    public bool IsSessionValid(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
        if (session == null) return false;

        return session.Time.AddMinutes(20) >= DateTime.UtcNow;
    }
}