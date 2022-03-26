using System.Text.Json;
using DB.Entities;
using DB.Repositories.Computers;
using Microsoft.AspNetCore.Mvc;
using WebClient.Models;

namespace WebClient.Controllers;

public class ComputerController : Controller
{
    private readonly IComputerRepository _computerRepository;

    public ComputerController()
    {
        _computerRepository = new ComputerEFRepository();
        //_computerRepository = new ComputerDapperRepository();
        //_computerRepository = new ComputerDefaultRepository();
    }

    [HttpPost]
    [Route("ParseComp")]
    public string ParseComp([FromBody] JsonElement json)
    {
        var responceObj = new ResponceObject<Computer>();
        string responceJson;

        var isValid = Utils.Util.CheckToken(null, HttpContext.Request.Cookies);
        if (!isValid)
        {
            responceObj.Access = 1;
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
        }

        if (json.TryGetProperty("id", out var jsonElementID))
        {
            var id = jsonElementID.GetUInt32();
            return GetComputer(id);
        }

        if (json.TryGetProperty("list", out _))
        {
            return GetComputers(json);
        }

        if (json.TryGetProperty("comp", out var jsonElementComp))
        {
            return ParseComp(jsonElementComp, responceObj);
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    private string GetComputer(uint id)
    {
        var responceObj = new ResponceObject<Computer>();
        string responceJson;

        var computer = _computerRepository.GetItem(id);
        if (computer != null)
        {
            responceObj.Success = 1;
            responceObj.Data = computer;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    private string GetComputers(JsonElement json)
    {
        string? name = null;
        uint status = 0;
        uint employerID = 0;
        DateTime? date = null;
        string? cpu = null;
        decimal price = 0;

        var responceObj = new ResponceObject<Computer>();
        string responceJson;

        if (json.TryGetProperty("name", out var nameElement))
        {
            name = nameElement.GetString();
        }
        if (json.TryGetProperty("status", out var statusElement))
        {
            status = statusElement.GetUInt32();
        }
        if (json.TryGetProperty("employerID", out var employerIDElement))
        {
            employerID = employerIDElement.GetUInt32();
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

        var computers = _computerRepository.GetFilterItems(name, status, employerID, date, cpu, price);

        if (computers.Count > 0)
        {
            responceObj.Success = 1;
            responceObj.DataList = computers;
        }

        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
    }

    private string ParseComp(JsonElement jsonElementComp, ResponceObject<Computer> responceObj)
    {
        string responceJson;
        Computer? computer = null;

        if (jsonElementComp.TryGetProperty("id", out var idElement))
        {
            var id = idElement.GetUInt32();
            computer = _computerRepository.GetItem(id);
            if (computer == null)
            {
                responceJson = Utils.Util.SerializeToJson(responceObj);
                return responceJson;
            }
        }

        if (jsonElementComp.TryGetProperty("del", out _))
        {
            if (computer != null)
            {
                var success = _computerRepository.DeleteItem(computer.ID);
                if (success)
                {
                    responceObj.Success = 1;
                }
            }
            responceJson = Utils.Util.SerializeToJson(responceObj);
            return responceJson;
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
        if (jsonElementComp.TryGetProperty("employerID", out var employerIDElement))
        {
            computer.EmployerID = employerIDElement.GetUInt32();
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
            _computerRepository.CreateItem(computer);
            responceObj.Success = 1;
            responceObj.Data = computer;
        }
        else
        {
            var success = _computerRepository.UpdateItem(computer);
            if (success)
            {
                responceObj.Success = 1;
            }
        }
        responceJson = Utils.Util.SerializeToJson(responceObj);
        return responceJson;
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


