using System.Text.Json;
using DB;
using DB.Entities;
using DB.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebClient.Controllers;

public class SessionController : ControllerBase
{
    private readonly ApplicationContextEF _db;
    public SessionController()
    {
        var opt = new DbContextOptionsBuilder<ApplicationContextEF>();
        opt.UseMySql(MySQLDatabaseContext.ConnectionString, ApplicationContextEF.ServerVersion);
        _db = new ApplicationContextEF(opt.Options);
    }


    [HttpGet]
    [Route("login")]
    public dynamic Login([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        string? token;
        Session? session = null;
        var responceErrObj = new
        {
            success = 0
        };

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();

            if (string.IsNullOrEmpty(token)) return responceErrObj;

            session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
            if (session != null)
            {
                var isValid = session.Time.AddMinutes(20) > DateTime.UtcNow;
                if (!isValid)
                {
                    session.IsDeleted = true;
                    _db.Session.Update(session);
                    _db.SaveChanges();
                    return responceErrObj;

                }
            }
        }
        else
        {
            if (json.TryGetProperty("login", out var loginElement))
            {
                login = loginElement.GetString();
            }

            if (json.TryGetProperty("password", out var passwordElement))
            {
                password = passwordElement.GetString();
                password = Util.Encode(password);
            }

            if (string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password)) return responceErrObj;

            var user = _db.Users.FirstOrDefault(p => p.Login.Equals(login));
            if (user != null &&
                (user.Pass.Equals(password)))
            {
                session = new Session
                {
                    Token = Guid.NewGuid().ToString("D"),
                    Time = DateTime.UtcNow,
                    UserID = user.ID,
                    UserIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                _db.Session.Add(session);
                _db.SaveChanges();
            }
        }

        if (session != null)
        {
            var responceOkObj = new
            {
                success = 1,
                token = session.Token
            };
            return JsonSerializer.Serialize(responceOkObj);

        }
        return responceErrObj;
    }

    [HttpGet]
    [Route("GetMe")]
    public dynamic GetMe([FromBody] JsonElement json)
    {
        string? token = null;
        var responceErrObj = new
        {
            success = 0
        };

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }

        var session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
        if (session == null) return responceErrObj;

        var user = _db.Users.FirstOrDefault(p => p.ID == session.UserID);
        if (user == null) return responceErrObj;

        var responceOkObj = new
        {
            login = user.Login,
            employerId = user.EmployerId
        };
        return responceOkObj;

    }

    [HttpPost]
    [Route("Registration")]
    public dynamic Registration([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        var responceErrObj = new
        {
            success = 0
        };

        var responceOkObj = new
        {
            success = 1
        };


        if (json.TryGetProperty("login", out var loginElement))
        {
            login = loginElement.GetString();
        }

        if (json.TryGetProperty("password", out var passwordElement))
        {
            password = passwordElement.GetString();
            password = Util.Encode(password);
        }

        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password)) return responceErrObj;

        var check = _db.Users.FirstOrDefault(p => p.Login.Equals(login));
        if (check != null) return responceErrObj;

        var user = new User() { Login = login, Pass = password, EmployerId = 0 };
        _db.Users.Add(user);
        _db.SaveChanges();
        return responceOkObj;
    }

    [HttpPost]
    [Route("LogOut")]
    public dynamic Delete([FromBody] JsonElement json)
    {
        string? token = null;
        var responceErrObj = new
        {
            success = 0
        };

        var responceOkObj = new
        {
            success = 1
        };

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }

        if (string.IsNullOrEmpty(token)) return responceErrObj;

        var session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
        if (session == null)
        {
            return responceErrObj;
        }
        _db.Session.Update(session);
        _db.SaveChanges();

        return responceOkObj;
    }
}