using System.Text.Json;
using DB.Entities;
using DB.Repositories;
using DB.Repositories.Computer;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    [Route("[controller]")]
    public class ComputerController : Controller
    {
        private readonly IComputerRepository _computerRepository;

        public ComputerController()
        {
            _computerRepository = new ComputerRepositoryDefault();
        }

        [HttpPost]
        public dynamic ComputerTest([FromBody] JsonElement json)
        {
            var errorObj = new
            {
                success = 0
            };

            if (json.TryGetProperty("id", out var jsonElementId))
            {
                var id = jsonElementId.GetInt32();
                return GetComputer(id);
            }
            if (json.TryGetProperty("list", out var jsonElementList))
            {
                return GetComputers();
            }
            if (json.TryGetProperty("comp", out var jsonElementComp))
            {
                return ParseComp(jsonElementComp, errorObj);
            }
            return JsonSerializer.Serialize(errorObj);
        }

        private dynamic ParseComp(JsonElement jsonElementComp, dynamic errorObj)
        {
            Computer? computer = null;

            if (jsonElementComp.TryGetProperty("id", out var idElement))
            {
                var id = idElement.GetInt32();
                computer = _computerRepository.GetComputer(id);
                if (computer == null) return JsonSerializer.Serialize(errorObj);
            }

            if (jsonElementComp.TryGetProperty("del", out var delElement))
            {
                if (computer == null) return JsonSerializer.Serialize(errorObj);

                var result = _computerRepository.DeleteComputer(computer.Id);
                var resultObj = new
                {
                    success = result
                };
                return JsonSerializer.Serialize(resultObj);
            }

            if (computer == null)
            {
                computer = new Computer();
            }

            if (jsonElementComp.TryGetProperty("name", out var nameElement))
            {
                computer.Name = nameElement.GetString();
            }
            if (jsonElementComp.TryGetProperty("status", out var statusElement))
            {
                computer.Status = statusElement.GetInt32();
            }
            if (jsonElementComp.TryGetProperty("employerId", out var employerIdElement))
            {
                computer.EmployerId = employerIdElement.GetInt32();
            }
            if (jsonElementComp.TryGetProperty("date", out var dateElement))
            {
                computer.Date = dateElement.GetDateTime();
            }
            if (jsonElementComp.TryGetProperty("cpu", out var cpuElement))
            {
                computer.Cpu = cpuElement.GetString();
            }
            if (jsonElementComp.TryGetProperty("price", out var priceElement))
            {
                computer.Price = priceElement.GetDecimal();
            }

            if (computer.Id == 0)
            {
                _computerRepository.CreateComputer(computer);
                var resultObj = new
                {
                    success = 1,
                    id = computer.Id
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

        private dynamic GetComputer(int id)
        {
            var computer = _computerRepository.GetComputer(id);
            if (computer != null)
            {
                var outComp = new
                {
                    name = computer.Name,
                    status = computer.Status,
                    employerId = computer.EmployerId,
                    date = computer.Date,
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

        private dynamic GetComputers()
        {
            var computers = _computerRepository.GetComputers();
            var outComputerList = new List<dynamic>();

            foreach (var computer in computers)
            {
                var outComp = new
                {
                    id = computer.Id,
                    name = computer.Name,
                    status = computer.Status,
                    employerId = computer.EmployerId,
                    date = computer.Date,
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
}


