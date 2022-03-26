using System.Net;
using System.Net.Mail;
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
       // _userRepository = new UserEFRepository();
       // _userRepository = new UserDefaultRepository();
       _userRepository = new UserDapperRepository();
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
                var isValid = session.Time.AddMinutes(20) > DateTime.UtcNow &&
                              !session.IsDeleted;
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

            if (user is {IsActivated: false})
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (user is { Password: { } } && (user.Password.Equals(password)))
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

        var isValid = Utils.Util.CheckToken(json, null);
        if (!isValid)
        {
            responceObj.Access = 1;
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
        string? userMail = null;
        string? confirmationСode = null;
        var random = new Random();
        var activationCode = random.Next(100000, 1000000).ToString();
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        if (json.TryGetProperty("login", out var loginElement))
        {
            login = loginElement.GetString();
        }

        if (json.TryGetProperty("email", out var mailElement))
        {
            userMail = mailElement.GetString();
        }

        if (json.TryGetProperty("password", out var passwordElement))
        {
            password = passwordElement.GetString();
            password = Util.Encode(password);
        }

        if (json.TryGetProperty("activationCode", out var codElement))
        {
            confirmationСode = codElement.GetString();
        }

        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(userMail) ||
            !Util.CheckEmail(userMail))
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var userFromDbByLogin = _userRepository.GetItem(login);
        if (userFromDbByLogin != null)
        {
            if (!string.IsNullOrEmpty(confirmationСode) &&
                !string.IsNullOrEmpty(userFromDbByLogin.ActivationCode) &&
                confirmationСode.Equals(userFromDbByLogin.ActivationCode))
            {
                userFromDbByLogin.IsActivated = true;
                userFromDbByLogin.ActivationCode = null;
                _userRepository.UpdateItem(userFromDbByLogin);

                responceObj.Success = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        //проверка на уникальность емейла
        var userFromDbByEmail = _userRepository.GetItemByEmail(userMail);
        if (userFromDbByEmail != null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var user = new User() { Login = login, Password = password, EmployerID = 0, Email = userMail, IsActivated = false, ActivationCode = activationCode };
        _userRepository.CreateItem(user);

        var from = new MailAddress("testlolkek1234@gmail.com", "Rendre");
        var to = new MailAddress(userMail);
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = "Регистрация";
        mailMessage.Body = $"<h2>Код подтверждения регистрации: {activationCode}</h2>";
        mailMessage.IsBodyHtml = true;
        var smtpClient = new SmtpClient("smtp.gmail.com", 587);
        smtpClient.Credentials = new NetworkCredential("testlolkek1234@gmail.com", "bytdxcursbiylsug");
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMessage);

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

        session.IsDeleted = true;
        _sessionRepository.UpdateItem(session);

        responceObj.Success = 1;
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}