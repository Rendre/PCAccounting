using System.Text.Json;
using DB.Repositories.User;
using DB.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController()
    {
        _userRepository = new UserDapperRepository();
    }

    [HttpGet("{id:int}")]
    public dynamic GetUser(uint id)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var user = _userRepository.GetItem(id);
        if (user != null)
        {
            var responseObj = new
            {
                success = 1,
                login = user.Login,
                employerId = user.EmployerId
            };
            return responseObj;

        }

        return responseErrObj;
    }
        
    [HttpGet]
    public dynamic GetUsers()
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var userList = _userRepository.GetItems();
        var outUserList = new List<dynamic>();
        foreach (var user in userList)
        {
            var outUser = new
            {
                id = user.ID,
                login = user.Login,
                employerId = user.EmployerId
            };
            outUserList.Add(outUser);
        }
        if (outUserList.Count > 0)
        {
            var responseObj = new
            {
                success = 1,
                users = outUserList
            };
            return responseObj;
        }
        else
        {
            var responseObj = new
            {
                success = 0
            };
            return responseObj;
        }
    }

    [HttpDelete("{id:int}")]
    public dynamic DeleteUser(uint id)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var deleteObjectCounts = _userRepository.DeleteItem(id);
        var resultObj = new
        {
            success = deleteObjectCounts
        };

        return resultObj;
    }

    [HttpPost]
    public dynamic CreateUser([FromBody] JsonElement userJsn)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var login = userJsn.GetProperty("login").GetString();
        var password = userJsn.GetProperty("password").GetString();
        password = Util.Encode(password);
        var employerId = userJsn.GetProperty("employerId").GetUInt32();
        var userId = _userRepository.CreateUser(login, password, employerId);
        if (userId <= 0)
        {
            return responseErrObj;
        }
        else
        {
            var resultObj = new
            {
                success = 1
            };
            return resultObj;
        }
    }

    [HttpPut]
    public dynamic ChangeUser([FromBody] JsonElement userJsn)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var id = userJsn.GetProperty("id").GetUInt32();
        var login = userJsn.GetProperty("login").GetString();
        var password = userJsn.GetProperty("password").GetString();
        password = Util.Encode(password);
        var employerId = userJsn.GetProperty("employerId").GetUInt32();
        var success = _userRepository.ChangeUser(id, login, password, employerId);
        return JsonSerializer.Serialize(success);
    }

}