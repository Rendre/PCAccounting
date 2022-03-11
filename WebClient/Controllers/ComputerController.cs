using System.Text.Json;
using DB.Entities;
using DB.Repositories.Computer;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

public class ComputerController : Controller
{
    private readonly IComputerRepository _computerRepository;

    public ComputerController()
    {
        _computerRepository = new ComputerEFRepository();
    }

    [HttpPost]
    [Route("ParseComp")]
    public dynamic ParseComp([FromBody] JsonElement json)
    {
        var responseErrObj = new
        {
            success = 0
        };
        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid) return responseErrObj;

        if (json.TryGetProperty("id", out var jsonElementId))
        {
            var id = jsonElementId.GetUInt32();
            return GetComputer(id);
        }
        if (json.TryGetProperty("list", out _))
        {
            return GetComputers(json);
        }
        if (json.TryGetProperty("comp", out var jsonElementComp))
        {
            return ParseComp(jsonElementComp, responseErrObj);
        }
        return JsonSerializer.Serialize(responseErrObj);
    }

    private dynamic ParseComp(JsonElement jsonElementComp, dynamic errorObj)
    {
        Computer? computer = null;

        if (jsonElementComp.TryGetProperty("id", out var idElement))
        {
            var id = idElement.GetUInt32();
            computer = _computerRepository.GetComputer(id);
            if (computer == null) return JsonSerializer.Serialize(errorObj);
        }

        if (jsonElementComp.TryGetProperty("del", out _))
        {
            if (computer == null) return JsonSerializer.Serialize(errorObj);

            var result = _computerRepository.DeleteComputer(computer.ID);
            var resultObj = new
            {
                success = result
            };
            return JsonSerializer.Serialize(resultObj);
        }

        computer ??= new Computer();

        if (jsonElementComp.TryGetProperty("name", out var nameElement))
        {
            computer.Name = nameElement.GetString();
        }
        if (jsonElementComp.TryGetProperty("status", out var statusElement))
        {
            computer.StatusID = statusElement.GetUInt32();
        }
        if (jsonElementComp.TryGetProperty("employerId", out var employerIdElement))
        {
            computer.EmployerId = employerIdElement.GetUInt32();
        }
        if (jsonElementComp.TryGetProperty("date", out var dateElement))
        {
            computer.DateCreated = dateElement.GetDateTime();
        }
        if (jsonElementComp.TryGetProperty("cpu", out var cpuElement))
        {
            computer.Cpu = cpuElement.GetString();
        }
        if (jsonElementComp.TryGetProperty("price", out var priceElement))
        {
            computer.Price = priceElement.GetDecimal();
        }

        if (computer.ID == 0)
        {
            _computerRepository.CreateComputer(computer);
            var resultObj = new
            {
                success = 1,
                id = computer.ID
            };
            return JsonSerializer.Serialize(resultObj);
        }
        else
        {
            var result = _computerRepository.ChangeComputer(computer);
            var resultObj = new
            {
                success = result
            };
            return JsonSerializer.Serialize(resultObj);
        }
    }

    private dynamic GetComputer(uint id)
    {
        var computer = _computerRepository.GetComputer(id);
        if (computer != null)
        {
            var outComp = new
            {
                name = computer.Name,
                status = computer.StatusID,
                employerId = computer.EmployerId,
                date = computer.DateCreated,
                cpu = computer.Cpu,
                price = computer.Price
            };
            var responseObj = new
            {
                success = 1,
                comp = outComp

            };
            return JsonSerializer.Serialize(responseObj);
        }
        else
        {
            var responseObj = new
            {
                success = 0

            };
            return JsonSerializer.Serialize(responseObj);
        }
    }

    private dynamic GetComputers(JsonElement json)
    {
        string? name = null;
        uint status = 0;
        uint employerId = 0;
        DateTime? date = null;
        string? cpu = null;
        decimal price = 0;

        if (json.TryGetProperty("name", out var nameElement))
        {
            name = nameElement.GetString();
        }
        if (json.TryGetProperty("status", out var statusElement))
        {
            status = statusElement.GetUInt32();
        }
        if (json.TryGetProperty("employerId", out var employerIdElement))
        {
            employerId = employerIdElement.GetUInt32();
        }
        if (json.TryGetProperty("date", out var dateElement))
        {
            date = dateElement.GetDateTime();
        }
        if (json.TryGetProperty("cpu", out var cpuElement))
        {
            cpu = cpuElement.GetString();
        }
        if (json.TryGetProperty("price", out var priceElement))
        {
            price = priceElement.GetDecimal();
        }

        var computers = _computerRepository.GetFilterComputers(name, status, employerId, date, cpu, price);
        var outComputerList = new List<dynamic>();

        foreach (var computer in computers)
        {
            var outComp = new
            {
                id = computer.ID,
                name = computer.Name,
                status = computer.StatusID,
                employerId = computer.EmployerId,
                date = computer.DateCreated,
                cpu = computer.Cpu,
                price = computer.Price
            };
            outComputerList.Add(outComp);
        }

        var responseObj = new
        {
            success = 1,
            comps = outComputerList
        };
        return JsonSerializer.Serialize(responseObj);
    }

}


//private dynamic GetComputers()
//{
//    var computers = _computerRepository.GetComputers();
//    var outComputerList = new List<dynamic>();

//    foreach (var computer in computers)
//    {
//        var outComp = new
//        {
//            id = computer.ID,
//            name = computer.Name,
//            status = computer.StatusID,
//            employerId = computer.EmployerId,
//            date = computer.DateCreated,
//            cpu = computer.Cpu,
//            price = computer.Price
//        };
//        outComputerList.Add(outComp);
//    }

//    var responseObj = new
//    {
//        success = 1,
//        comps = outComputerList
//    };
//    return JsonSerializer.Serialize(responseObj);
//}


