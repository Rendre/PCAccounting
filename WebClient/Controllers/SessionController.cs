using System.Text.Json;
using DB.Entities;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services.LoginService;
using SharedKernel.Utils;
using WebClient.Models;

namespace WebClient.Controllers;

public class SessionController : ControllerBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FileController> _logger;
    private readonly ILoginService _loginService;

    public SessionController(ILogger<FileController> logger, ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _loginService = new LoginService(_sessionRepository, _userRepository);
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
                session = _loginService.GetSession(token);
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

                var userIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                session = _loginService.GetNewSession(login, password, userIP!);
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
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpGet]
    [Route("GetMe")]
    public string GetMe([FromBody] JsonElement json)
    {
        string? token = null;
        var responceUserObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            if (json.TryGetProperty("token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }

            var user = _loginService.GetUser(token);
            if (user != null)
            {
                responceUserObj = new ResponceObject<User>
                {
                    Success = 1,
                    Data = user
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

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
        var responceObj = new ResponceObject<Session>();
        string responceJson;

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

                isSuccess = _loginService.AccountActivation(login, userMail, confirmationСode);
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

            isSuccess = _loginService.CreateNewAccount(login, password, userMail);
            if (isSuccess)
            {
                responceObj.Success = 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

        try
        {
            if (json.TryGetProperty("token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }

            var session = _loginService.GetSession(token);
            if (session != null)
            {
                session.IsDeleted = true;
                _sessionRepository.SaveItem(session);
            }

            responceObj.Success = 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        var responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}