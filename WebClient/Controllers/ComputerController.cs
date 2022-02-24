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
            _computerRepository = new ComputerRepository();
        }

        [HttpPost]
        public dynamic ComputerTest([FromBody] JsonElement json)
        {
            if (json.TryGetProperty("id", out var jsonElementId))
            {
                var id = jsonElementId.GetInt32();
                var computer = GetComputer(id);
            }

            if (json.TryGetProperty("list", out var jsonElementList))
            {
                var list = GetComputers();
            }

            if (json.TryGetProperty("comp", out var jsonElementComp))
            {
                int id = 0;
                int del = 0;
                string name = "";

                if (jsonElementComp.TryGetProperty("id", out var idElement))
                {
                    id = idElement.GetInt32();
                }
                if (jsonElementComp.TryGetProperty("del", out var delElement))
                {
                    del = delElement.GetInt32();
                }
                if (jsonElementComp.TryGetProperty("name", out var nameElement))
                {
                    name = nameElement.GetString();
                }

                // создание
                if (id == 0 && 
                    del == 0 && 
                    name != null)
                {
                    return CreateComputer(jsonElementComp, name);
                }

                // изменение
                if (id > 0 && 
                    del == 0)
                {
                    return ChangeComputer(jsonElementComp, id);
                }

                //delete
                if (id > 0 && 
                    del != 0)
                {
                    DeleteComputer(id);
                }
            }
            return "";
        }

        private dynamic CreateComputer(JsonElement jsonElementComp, string name)
        {
            var computer = new Computer() {Name = name};

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

            _computerRepository.CreateComputer(computer);

            if (computer.Id <= 0)
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
                    id = computer.Id
                };
                return JsonSerializer.Serialize(responceObj);

            }
        }

        private dynamic ChangeComputer(JsonElement jsonElementComp, int id)
        {
            var computer = _computerRepository.GetComputer(id);

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

            var success = _computerRepository.ChangeComputer(computer);
            if (success > 0)
            {
                var resultObj = new
                {
                    Success = success,
                };
                return JsonSerializer.Serialize(resultObj);
            }
            else
            {
                var resultObj = new
                {
                    Success = success,
                };
                return JsonSerializer.Serialize(resultObj);
            }
        }

        private static dynamic GetComputer(int id)
        {
            return "";
        }

        private static dynamic GetComputers()
        {
            return "";
        }

        private static dynamic DeleteComputer(int id)
        {
            return "";
        }
    }
}
