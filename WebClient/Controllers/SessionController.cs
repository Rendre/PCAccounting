using System.Net;
using System.Net.Mail;
using System.Text.Json;
using DB;
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
    private readonly ProjectProperties _projectProperties;
    private readonly ILogger<FileController> _logger;

    public SessionController(ILogger<FileController> logger, ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _projectProperties = Util.GetProjectProperties()!;
    }

    public Session? Kek(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var session = _sessionRepository.GetItems(token, take: 1).FirstOrDefault();
        if (session == null) return null;

        var isValid = session.Time.AddMinutes(20) > DateTime.UtcNow &&
                      !session.IsDeleted;
        if (isValid) return session;

        session.IsDeleted = true;
        _sessionRepository.SaveItem(session);
        return null;
    }

    public Session? Kekw(string? login, string? password)
    {
        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password)) return null;

        password = Util.Encode(password);
        var user = _userRepository.GetItems(login, isActivated: EntityStatus.OnlyActive, take: 1).FirstOrDefault();
        if (user is { IsActivated: false }) return null;

        if (user is { Password: { } } &&
            (user.Password.Equals(password)))
        {
            var session = new Session
            {
                Token = Guid.NewGuid().ToString("D"),
                Time = DateTime.UtcNow,
                UserID = user.ID,
                UserIP = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _sessionRepository.SaveItem(session);
            return session;
        }

        return null;
    }

    [HttpGet]
    [Route("login")]
    public string Login([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        try
        {
            Session? session;
            if (json.TryGetProperty("token", out var tokenElement))
            {
                var token = tokenElement.GetString();
                session = Kek(token);
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
                }

                session = Kekw(login, password);
            }

            if (session != null)
            {
                responceObj.Success = 1;
                responceObj.Data = session;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
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
        var responceUserObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            if (json.TryGetProperty("token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }

            var sessions = _sessionRepository.GetItems(token, take: 1);
            var session = sessions.FirstOrDefault();
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
            responceUserObj = new ResponceObject<User>
            {
                Success = 1,
                Data = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            responceJson = Utils.Util.SerializeToJson(responceUserObj);
            return responceJson;
        }

        responceJson = Utils.Util.SerializeToJson(responceUserObj);
        return responceJson;
    }

    public bool Qwe(string? login, string? userMail, string? confirmationСode, User? user)
    {
        if (string.IsNullOrEmpty(confirmationСode)) return false;

        user = _userRepository.GetItems(login, userMail, true).FirstOrDefault();
        if (user != null &&
            !string.IsNullOrEmpty(user.ActivationCode) &&
            confirmationСode.Equals(user.ActivationCode))
        {
            user.IsActivated = true;
            user.ActivationCode = null;
            _userRepository.SaveItem(user);
            return true;
        }

        return false;
    }

    public bool Ewq(string? login, string? password, string? userMail, User? user)
    {
        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(userMail) ||
            !Util.CheckEmail(userMail)) return false;

        user = _userRepository.GetItems(userMail, take: 1).FirstOrDefault();
        if (user != null) return false;

        var random = new Random();
        var activationCode = random.Next(100000, 1000000).ToString();
        user = new User
        {
            Login = login,
            Password = password,
            EmployerID = 0,
            Email = userMail,
            IsActivated = false,
            ActivationCode = activationCode
        };
        _userRepository.SaveItem(user);

        var from = new MailAddress(_projectProperties.MyEmail, _projectProperties.SendersName);
        var to = new MailAddress(userMail);
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = "Регистрация";
        mailMessage.Body = $"<h2>Код подтверждения регистрации: {activationCode}</h2>";
        mailMessage.IsBodyHtml = true;
        var smtpClient = new SmtpClient(_projectProperties.AddressSMTP, _projectProperties.PortSMTP);
        smtpClient.Credentials =
            new NetworkCredential(_projectProperties.MyEmail, _projectProperties.PasswordForMyEmail);
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMessage);

        return true;
    }

    [HttpPost]
    [Route("Registration")]
    public string Registration([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        string? userMail = null;
        // перенести, если пришел код - просто активируй и все. не доставай пароль и емейл
        //активировать можно либо по логину либо по мейлу
        var responceObj = new ResponceObject<Session>();
        string responceJson;
        User? user = null;

        try
        {
            if (json.TryGetProperty("login", out var loginElement))
            {
                login = loginElement.GetString();
            }

            if (json.TryGetProperty("email", out var mailElement))
            {
                userMail = mailElement.GetString();
            }

            bool isSuccess;
            if (json.TryGetProperty("activationCode", out var codElement))
            {
                var confirmationСode = codElement.GetString();

                isSuccess = Qwe(login, userMail, confirmationСode, user);
                if (isSuccess)
                {
                    responceObj.Success = 1;
                }
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (json.TryGetProperty("password", out var passwordElement))
            {
                password = passwordElement.GetString();
                password = Util.Encode(password);
            }

            isSuccess = Ewq(login, password, userMail, user);
            if (isSuccess)
            {
                responceObj.Success = 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

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

        try
        {
            if (json.TryGetProperty("token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }

            if (string.IsNullOrEmpty(token))
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var sessions = _sessionRepository.GetItems(token, take: 1);
            var session = sessions.FirstOrDefault();
            if (session == null)
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            session.IsDeleted = true;
            _sessionRepository.SaveItem(session);

            responceObj.Success = 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}