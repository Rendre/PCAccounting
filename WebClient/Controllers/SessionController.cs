using System.Text.Json;
using DB.Entities;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Utils;
using WebClient.Models;

namespace WebClient.Controllers;

public class SessionController : ControllerBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    public SessionController()
    {
        _sessionRepository = new SessionEFRepository();
        _userRepository = new UserEFRepository();
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

            session = _sessionRepository.GetItem(token);
            if (session != null)
            {
                var isValid = session.Time.AddMinutes(20) > DateTime.UtcNow;
                if (!isValid)
                {
                    session.IsDeleted = true;
                    _sessionRepository.UpdateItem(session);
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

            var user = _userRepository.GetItem(login);
            if (user is {Password: { }} && (user.Password.Equals(password)))
            {
                session = new Session
                {
                    Token = Guid.NewGuid().ToString("D"),
                    Time = DateTime.UtcNow,
                    UserID = user.ID,
                    UserIP = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                _sessionRepository.CreateItem(session);
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

        if (string.IsNullOrEmpty(token))
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var session = _sessionRepository.GetItem(token);
        if (session == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var user = _userRepository.GetItem(session.UserID);
        if (user == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        user.Password = null;
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
        var userFromDb = _userRepository.GetItem(login);
        if (userFromDb != null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var user = new User() { Login = login, Password = password, EmployerID = 0 };
        _userRepository.CreateItem(user);

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

        var session = _sessionRepository.GetItem(token);
        if (session == null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
        _sessionRepository.UpdateItem(session);

        responceObj.Success = 1;
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}