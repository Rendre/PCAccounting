using System.Text.Json;
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

    public UserController()
    {
        _userRepository = new UserDapperRepository();
    }

    [HttpPost]
    public string CreateUser([FromBody] JsonElement userJsn)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

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
        var employerId = userJsn.GetProperty("employerId").GetUInt32();
        var user = new User() {Login = login, Pass = password, EmployerId = employerId};
        _userRepository.CreateItem(user);
        if (user.ID > 0)
        {
            responceObj.Success = 1;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpPut]
    public string ChangeUser([FromBody] JsonElement userJsn)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var id = userJsn.GetProperty("id").GetUInt32();
        var login = userJsn.GetProperty("login").GetString();
        var password = userJsn.GetProperty("password").GetString();
        password = Util.Encode(password);
        var employerId = userJsn.GetProperty("employerId").GetUInt32();
        var success = _userRepository.ChangeItem(id, login, password, employerId);

        if (success)
        {
            responceObj.Success = 1;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpGet("{id:int}")]
    public string GetUser(uint id)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

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
            user.Pass = null;
            responceObj.Data = user;
            responceObj.Success = 1;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpGet]
    public string GetUsers()
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var userList = _userRepository.GetItems();
        foreach (var user in userList)
        {
            user.Pass = null;
        }
        if (userList.Count > 0)
        {
            responceObj.Success = 1;
            responceObj.DataList = userList;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpDelete("{id:int}")]
    public string DeleteUser(uint id)
    {
        var responceObj = new ResponceObject<User>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var deleteObjectCounts = _userRepository.DeleteItem(id);
        if (deleteObjectCounts > 0)
        {
            responceObj.Success = 1;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}