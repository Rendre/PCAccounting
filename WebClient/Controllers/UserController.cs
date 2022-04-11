using System.Text.Json;
using DB;
using DB.Entities;
using DB.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Utils;
using WebClient.Models;

namespace WebClient.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FileController> _logger;

    public UserController(ILogger<FileController> logger)
    {
        _logger = logger;
        _userRepository = new UserEFRepository();
        //_userRepository = new UserDapperRepository();
    }

    [HttpPost]
    public string CreateUser([FromBody] JsonElement userJsn)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var login = userJsn.GetProperty("login").GetString();
            var password = userJsn.GetProperty("password").GetString();
            password = Util.Encode(password);
            var employerID = userJsn.GetProperty("employerID").GetUInt32();
            var user = new User { Login = login, Password = password, EmployerID = employerID };

            _userRepository.SaveItem(user);
            if (user.ID > 0)
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

    [HttpPut]
    public string UpdateUser([FromBody] JsonElement userJsn)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            // редко используется
            var id = userJsn.GetProperty("id").GetUInt32();
            var login = userJsn.GetProperty("login").GetString();
            var password = userJsn.GetProperty("password").GetString();
            password = Util.Encode(password);
            var employerID = userJsn.GetProperty("employerID").GetUInt32();
            var user = new User { ID = id, Login = login, Password = password, EmployerID = employerID };

            var success = _userRepository.SaveItem(user);

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

    [HttpGet("{id:int}")]
    public string GetUser(uint id)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var user = _userRepository.GetItem(id);
            if (user != null)
            {
                user.Password = null;
                responceObj.Data = user;
            }

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

    [HttpGet]
    public string GetUsers([FromBody] JsonElement json)
    {
        string? login = null;
        uint employerID = 0;
        var entityStatus = EntityStatus.None;
        string? email = null;

        var responceObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            if (json.TryGetProperty("login", out var loginElement))
            {
                login = loginElement.GetString();
            }

            if (json.TryGetProperty("employerID", out var employerIDElement))
            {
                employerID = employerIDElement.GetUInt32();
            }

            if (json.TryGetProperty("entityStatus", out var statusElement))
            {
                entityStatus = statusElement.GetBoolean() ? EntityStatus.OnlyActive : EntityStatus.OnlyInactive;
            }

            if (json.TryGetProperty("email", out var emailElement))
            {
                email = emailElement.GetString();
            }

            var userList = _userRepository.GetItems(login, email, employerID, entityStatus);
            foreach (var user in userList)
            {
                user.Password = null;
            }

            responceObj.Success = 1;
            if (userList.Count > 0)
            {
                responceObj.DataList = userList;
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

    [HttpDelete("{id:int}")]
    public string DeleteUser(uint id)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        try
        {
            var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
            if (!isValid)
            {
                responceObj.Access = 1;
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            var userFromDb = _userRepository.GetItem(id);
            if (userFromDb == null)
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }

            userFromDb.IsDeleted = true;
            var success = _userRepository.SaveItem(userFromDb);
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

//[HttpGet]
//public string GetUsers()
//{
//    var responceObj = new ResponceObject<User>();
//    string responceJson;

//    var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
//    if (!isValid)
//    {
//        responceObj.Access = 1;
//        responceJson = Utils.Util.SerializeToJson(responceObj);
//        return responceJson;
//    }

//    var userList = _userRepository.GetItems(isActivated: EntityStatus.OnlyActive);
//    foreach (var user in userList)
//    {
//        user.Password = null;
//    }
//    if (userList.Count > 0)
//    {
//        responceObj.Success = 1;
//        responceObj.DataList = userList;
//    }

//    responceJson = Utils.Util.SerializeToJson(responceObj);
//    return responceJson;
//}