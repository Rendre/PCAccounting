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
    private readonly ProjectProperties _projectProperties;

    public SessionController()
    {
        _sessionRepository = new SessionEFRepository();
        // _userRepository = new UserEFRepository();
        // _userRepository = new UserDefaultRepository();
        _userRepository = new UserDapperRepository();
        _projectProperties = Util.GetProjectProperties()!;
    }

    //систему с разными правами пользователь модератор и админ
    // пользователь может только смотреть
    //модератор менять
    //админ может управлять юзерами

    //посмотреть можно ли самим ставить аннотацию на контроллеры и методы -для проверки залогинен ты или нет

    //все контроллеры обернуть в трай кетч и чтоб логер чонить срал про них

    // чем отличаются мои дапер репозитории от ЕФ ? там чот не так с ЕФ // тесты ? xunit // assert equeal

    //репозитории должны заинектится в контроллер
    //и всякие ай файл сейф - это мои сервисы
    [HttpGet]
    [Route("login1")]
    public string Login1([FromBody] JsonElement json)
    {

        GC.Collect();
        return "";
    }


    [HttpGet]
    [Route("login")]
    public string Login([FromBody] JsonElement json)
    {
        string? login = null;
        string? password = null;
        Session? session = null;
        var responceObj = new ResponceObject<Session>();
        string responceJson;

        if (json.TryGetProperty("token", out var tokenElement))
        {
            var token = tokenElement.GetString();

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
        // перенести, если пришел код - просто активируй и все. не доставай пароль и емейл
        //активировать можно либо по логину либо по мейлу
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

        if (json.TryGetProperty("activationCode", out var codElement))
        {
            User? userFromDb = null;
            var confirmationСode = codElement.GetString();

            if (!string.IsNullOrEmpty(login))
            {
                userFromDb = _userRepository.GetItem(login);
            }

            if (!string.IsNullOrEmpty(userMail) &&
                userFromDb == null)
            {
                userFromDb = _userRepository.GetItemByEmail(userMail);
            }

            if (userFromDb != null)
            {
                if (!string.IsNullOrEmpty(confirmationСode) &&
                    !string.IsNullOrEmpty(userFromDb.ActivationCode) &&
                    confirmationСode.Equals(userFromDb.ActivationCode))
                {
                    userFromDb.IsActivated = true;
                    userFromDb.ActivationCode = null;
                    _userRepository.UpdateItem(userFromDb);

                    responceObj.Success = 1;
                }
            }
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        if (json.TryGetProperty("password", out var passwordElement))
        {
            password = passwordElement.GetString();
            password = Util.Encode(password);
        }

        //либо логин либо пароль
        if (string.IsNullOrEmpty(login) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(userMail) ||
            !Util.CheckEmail(userMail))
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
        //сделать чтоб картинки скачивались - получить список всех картинок по id компьютера
        // приходит id картинки и я ее возвращаю result asp net
        // 2 - добавляю ulr картинки и качаю по url


        // сделать метод гет итемс с фильтрацией у юзера и с лимитом
        //сколько пропустить сколько взять и фильтрация - возвр лист
        //с фильтрацией - возвр кол-во

        //удалить нах, сделать это в 205 строке одним запросом и посмотреть что каунт > 0
        //проверка на уникальность емейла

        var userFromDbByEmail = _userRepository.GetItemByEmail(userMail);
        if (userFromDbByEmail != null)
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }
        // вынести в шеред кернел
        //вынести проверку на уникальный логин и емейл - отдельно

        //вынести в шеред кернел
        var random = new Random();
        var activationCode = random.Next(100000, 1000000).ToString();
        var user = new User { Login = login, Password = password, EmployerID = 0, Email = userMail, IsActivated = false, ActivationCode = activationCode };
        _userRepository.CreateItem(user);

        var from = new MailAddress(_projectProperties.MyEmail, _projectProperties.SendersName);
        var to = new MailAddress(userMail);
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = "Регистрация";
        mailMessage.Body = $"<h2>Код подтверждения регистрации: {activationCode}</h2>";
        mailMessage.IsBodyHtml = true;
        var smtpClient = new SmtpClient(_projectProperties.AddressSMTP, _projectProperties.PortSMTP);
        smtpClient.Credentials = new NetworkCredential(_projectProperties.MyEmail, _projectProperties.PasswordForMyEmail);
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