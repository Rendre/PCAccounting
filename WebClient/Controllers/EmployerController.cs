using System.Text.Json;
using DB.Entities;
using DB.Repositories.Employers;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;

namespace WebClient.Controllers;

[Route("[controller]")]
public class EmployerController : ControllerBase
{
    private readonly IEmployerRepository _employerRepository;

    public EmployerController()
    {
        _employerRepository = new EmployerDapperRepository();
    }

    [HttpPost]
    public string CreateEmployer([FromBody] JsonElement emp)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var name = emp.GetProperty("name").GetString();
        var position = emp.GetProperty("position").GetString();
        var tel = emp.GetProperty("tel").GetString();

        var employer = new Employer() { Name = name, Position = position, Tel = tel };

        _employerRepository.CreateItem(employer);
        if (employer.ID > 0)
        {
            responceObj.Success = 1;
            responceObj.Data = new Employer() { ID = employer.ID };
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpPut]
    public string UpdateEmployer([FromBody] JsonElement emp)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var id = emp.GetProperty("id").GetUInt32();
        var name = emp.GetProperty("name").GetString();
        var position = emp.GetProperty("position").GetString();
        var tel = emp.GetProperty("tel").GetString();
        // взять по id эмплоера из бд
        // те поля которые отличаются, не пустые не равны нулл.. засунуть в объект и апдейтнуть его
        var employer = new Employer() { ID = id, Name = name, Position = position, Tel = tel };

        var success = _employerRepository.UpdateItem(employer);
        if (success)
        {
            responceObj.Success = 1;
            responceObj.Data = new Employer() { ID = employer.ID };
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

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
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
        if (employerList.Count > 0)
        {
            responceObj.Success = 1;
            responceObj.DataList = employerList;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpGet("{id:int}")]
    public string GetEmployer(uint id)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var employer = _employerRepository.GetItem(id);
        if (employer != null)
        {
            responceObj.Success = 1;
            responceObj.Data = employer;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    [HttpDelete("{id:int}")]
    public string DeleteEmployer(uint id)
    {
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        var deleteObjCounts = _employerRepository.DeleteItem(id);
        if (deleteObjCounts > 0)
        {
            responceObj.Success = 1;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }
}