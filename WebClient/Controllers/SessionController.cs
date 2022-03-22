using System.Text.Json;
using DB;
using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Utils;
using WebClient.Models;

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
    public string Login([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        string? token;
        Session? session = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();

            if (string.IsNullOrEmpty(token))
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
            if (session != null)
            {
                var isValid = session.Time.AddMinutes(20) > DateTime.UtcNow;
                if (!isValid)
                {
                    session.IsDeleted = true;
                    _db.Session.Update(session);
                    _db.SaveChanges();
                    responceJson = Utils.Util.SerializeToJson(responceObj);
                    return responceJson;
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
                string.IsNullOrEmpty(password))
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

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
            responceObj.Success = 1;
            responceObj.Data = session;
        }
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpGet]
    [Route("GetMe")]
    public string GetMe([FromBody] JsonElement json)
    {
        string? token = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }

        var session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
        if (session == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var user = _db.Users.FirstOrDefault(p => p.ID == session.UserID);
        if (user == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        user.Pass = null;
        var responceUserObj = new ResponceObject<User>
        {
            Success = 1,
            Data = user
        };

        responceJson = Utils.Util.SerializeToJson(responceUserObj);
        return responceJson;
    }

    [HttpPost]
    [Route("Registration")]
    public string Registration([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

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
            string.IsNullOrEmpty(password))
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var check = _db.Users.FirstOrDefault(p => p.Login.Equals(login));
        if (check != null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var user = new User() { Login = login, Pass = password, EmployerId = 0 };
        _db.Users.Add(user);
        _db.SaveChanges();

        responceObj.Success = 1;
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpPost]
    [Route("LogOut")]
    public string LogOut([FromBody] JsonElement json)
    {
        string? token = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        if (json.TryGetProperty("token", out var tokenElement))
        {
            token = tokenElement.GetString();
        }

        if (string.IsNullOrEmpty(token))
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var session = _db.Session.FirstOrDefault(p => p.Token != null && p.Token.Equals(token));
        if (session == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
        _db.Session.Update(session);
        _db.SaveChanges();

        responceObj.Success = 1;
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}