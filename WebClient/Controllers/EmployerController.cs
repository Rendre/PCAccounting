using System.Text.Json;
using DB.Entities;
using DB.Repositories.Employers;
using DB.Repositories.Sessions;
using DB.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services.LoginService;
using SharedKernel.Utils;
using WebClient.Models;

namespace WebClient.Controllers;

[Route("[controller]")]
public class EmployerController : ControllerBase
{
    private readonly IEmployerRepository _employerRepository;
    private readonly ILogger<FileController> _logger;
    private readonly ILoginService _loginService;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    public EmployerController(ILogger<FileController> logger, ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _logger = logger;
        //_employerRepository = new EmployerDapperRepository();
        _employerRepository = new EmployerEFRepository();
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _loginService = new LoginService(_sessionRepository, _userRepository);
    }

    [HttpPost]
    public string CreateEmployer([FromBody] JsonElement emp)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        try
        {
            var token = Utils.Util.GetToken(null, HttpContext.Request.Cookies);
            var isValid = _loginService.IsSessionValid(token).isValid;
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var name = emp.GetProperty("name").GetString();
            var position = emp.GetProperty("position").GetString();
            var tel = emp.GetProperty("tel").GetString();
            tel = Util.CheckTelNumber(tel);
            var employer = new Employer { Name = name, Position = position, Tel = tel };

            _employerRepository.SaveItem(employer);
            if (employer.ID > 0)
            {
                responceObj.Success = 1;
                responceObj.Data = new Employer { ID = employer.ID };
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

    [HttpPut]
    public string UpdateEmployer([FromBody] JsonElement json)
    {
        var isChanged = false;
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        try
        {
            var token = Utils.Util.GetToken(null, HttpContext.Request.Cookies);
            var isValid = _loginService.IsSessionValid(token).isValid;
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (json.TryGetProperty("id", out var idElement))
            {
                var id = idElement.GetUInt32();
                if (id == 0) return Utils.Util.SerializeToJson(responceObj);

                var employer = _employerRepository.GetItem(id);
                if (employer == null) return Utils.Util.SerializeToJson(responceObj);

                if (json.TryGetProperty("name", out var nameElement))
                {
                    var name = nameElement.GetString();
                    if (employer.Name != name)
                    {
                        employer.Name = name;
                        isChanged = true;
                    }
                }

                if (json.TryGetProperty("position", out var positionElement))
                {
                    var position = positionElement.GetString();
                    if (employer.Position != position)
                    {
                        employer.Position = position;
                        isChanged = true;
                    }
                }

                if (json.TryGetProperty("tel", out var telElement))
                {
                    var tel = telElement.GetString();
                    tel = Util.CheckTelNumber(tel);
                    if (!string.IsNullOrEmpty(tel))
                    {
                        if (employer.Tel != tel)
                        {
                            employer.Tel = tel;
                            isChanged = true;
                        }
                    }
                }

                if (isChanged)
                {
                    var success = _employerRepository.SaveItem(employer);
                    if (success)
                    {
                        responceObj.Success = 1;
                        responceObj.Data = employer;
                    }
                }
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
    public string GetEmployer([FromBody] JsonElement json)
    {
        string? name = null;
        string? position = null;
        string? tel = null;

        var responceObj = new ResponceObject<Employer>();
        string responceJson;
        
        try
        {
            var token = Utils.Util.GetToken(null, HttpContext.Request.Cookies);
            var isValid = _loginService.IsSessionValid(token).isValid;
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (json.TryGetProperty("name", out var nameElement))
            {
                name = nameElement.GetString();
            }

            if (json.TryGetProperty("position", out var positionElement))
            {
                position = positionElement.GetString();
            }

            if (json.TryGetProperty("tel", out var telElement))
            {
                tel = telElement.GetString();
            }

            var employerList = _employerRepository.GetItems(name, position, tel);

            responceObj.Success = 1;
            responceObj.DataList = employerList;
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

    [HttpGet("{id:int}")]
    public string GetEmployer(uint id)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        try
        {
            var token = Utils.Util.GetToken(null, HttpContext.Request.Cookies);
            var isValid = _loginService.IsSessionValid(token).isValid;
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var employer = _employerRepository.GetItem(id);

            responceObj.Success = 1;
            responceObj.Data = employer;

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

    [HttpDelete("{id:int}")]
    public string DeleteEmployer(uint id)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        try
        {
            var token = Utils.Util.GetToken(null, HttpContext.Request.Cookies);
            var isValid = _loginService.IsSessionValid(token).isValid;
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var employer = _employerRepository.GetItem(id);
            if (employer == null)
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            employer.IsDeleted = true;
            var success = _employerRepository.SaveItem(employer);
            if (success)
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
}