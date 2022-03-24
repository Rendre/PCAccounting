using System.Text.Json;
using DB.Entities;
using DB.Repositories.Employers;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Utils;
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
        Employer employer;
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        try
        {
            var name = emp.GetProperty("name").GetString();
            var position = emp.GetProperty("position").GetString();
            var tel = emp.GetProperty("tel").GetString();
            tel = Util.CheckTelNumber(tel); 
            employer = new Employer() { Name = name, Position = position, Tel = tel };
        }
        catch
        {
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

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
    public string UpdateEmployer([FromBody] JsonElement json)
    {
        var isChanged = false;
        var responceObj = new ResponceObject<Employer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
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
                var success = _employerRepository.UpdateItem(employer);
                if (success)
                {
                    responceObj.Success = 1;
                    responceObj.Data = employer;
                }
            }
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