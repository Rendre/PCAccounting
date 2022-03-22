using System.Text.Json;
using DB.Entities;
using DB.Repositories.Employer;
using Microsoft.AspNetCore.Mvc;

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
    public dynamic CreateEmployer([FromBody] JsonElement emp)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var name = emp.GetProperty("name").GetString();
        var position = emp.GetProperty("position").GetString();
        var tel = emp.GetProperty("tel").GetString();

        var employer = new Employer() {Name = name, Position = position, Tel = tel};

        _employerRepository.CreateItem(employer);
        if (employer.ID <= 0)
        {
            var responceObj = new
            {
                success = 0
            };
            return JsonSerializer.Serialize(responceObj);

        }
        else
        {
            var responceObj = new
            {
                success = 1,
                id = employer.ID
            };
            return JsonSerializer.Serialize(responceObj);

        }
    }

    [HttpPut]
    public dynamic СhangeEmployer([FromBody] JsonElement emp)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var id = emp.GetProperty("id").GetUInt32();
        var name = emp.GetProperty("name").GetString();
        var position = emp.GetProperty("position").GetString();
        var tel = emp.GetProperty("tel").GetString();
        var employer = new Employer() {ID = id, Name = name, Position = position, Tel = tel };

        var success = _employerRepository.ChangeItem(employer);
        if (success)
        {
            var resultObj = new
            {
                Success = success,
                Id = id,
            };
            return JsonSerializer.Serialize(resultObj);
        }
        else
        {
            var resultObj = new
            {
                Success = success,
                Id = id,
            };
            return JsonSerializer.Serialize(resultObj);
        }
    }

    [HttpDelete("{id:int}")]
    public dynamic DeleteEmployer(uint id)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var deleteObjCounts = _employerRepository.DeleteItem(id);
        var responceObj = new
        {
            success = deleteObjCounts
        };
        return JsonSerializer.Serialize(responceObj);
    }

    [HttpGet]
    public dynamic GetEmployer([FromBody] JsonElement json)
    {
        string? name = null;
        string? position = null;
        string? tel = null;
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

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
        var outEmployerList = new List<dynamic>();
        foreach (var employer in employerList)
        {
            var outEmployer = new
            {
                id = employer.ID,
                name = employer.Name,
                position = employer.Position,
                tel = employer.Tel
            };
            outEmployerList.Add(outEmployer);
        }
        if (outEmployerList.Count > 0)
        {
            var responceObj = new
            {
                success = 1,
                employers = outEmployerList
            };
            return JsonSerializer.Serialize(responceObj);
        }
        else
        {
            var responceObj = new
            {
                success = 0,
            };
            return JsonSerializer.Serialize(responceObj);
        }
    }



    [HttpGet("{id:int}")]
    public dynamic GetEmployer(uint id)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        var employer = _employerRepository.GetItem(id);
        if (employer != null)
        {
            var responceObj = new
            {
                success = 1,
                name = employer.Name,
                position = employer.Position,
                tel = employer.Tel
            };
            return JsonSerializer.Serialize(responceObj);
        }
        else
        {
            var responceObj = new
            {
                success = 0
            };
            return JsonSerializer.Serialize(responceObj);
        }
    }

}