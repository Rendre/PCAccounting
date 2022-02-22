using System.Text.Json;
using DB.Entities;
using DB.Repositories;
using Microsoft.AspNetCore.Mvc;
using EmployerRepository = DB.Repositories.EmployerRepository;

namespace WebClient.Controllers
{
    [Route("[controller]")]
    public class EmployerController : ControllerBase
    {
        /*public IResult Index()
        {
            var computer = new Computer() { Name = "comp1", Status = 1 };
            var json = Results.Json(computer);
            return json;
        }

        public dynamic Second()
        {
            var computer = new Computer() { Name = "comp2" };
            return computer;
        }

        public string Third()
        {
            var computer = new Computer() { Name = "tretii" };
            var jsonString = JsonSerializer.Serialize(computer);
            return jsonString;
        }

        public dynamic Forth()
        {
            var userRepository = new UserRepository();
            var user = userRepository.GetItem(5);
            var userMap = new Dictionary<string, object>();
            userMap["Id"] = user.Id;
            userMap["name"] = user.Login;
            userMap["Password"] = user.Password;
            userMap["EmployerId"] = user.EmployerId;

            var json = JsonSerializer.Serialize(userMap);

            return json;
        }*/

        [HttpPost]
        public dynamic Employer([FromBody] JsonElement emp)
        {
            var name = emp.GetProperty("name").GetString();
            var position = emp.GetProperty("position").GetString();
            var tel = emp.GetProperty("tel").GetString();
            var employerRepository = new EmployerRepository();
            var empId = employerRepository.CreateEmployer(name, position, tel);
            if (empId <= 0)
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
                    id = empId
                };
                return JsonSerializer.Serialize(responceObj);

            }
        }
        //чо ругается на сигнатуру если разные типы запросов на методах?
        [HttpPut]
        public dynamic PutEmployer([FromBody] JsonElement emp)
        {
            var id = emp.GetProperty("id").GetInt32();
            var name = emp.GetProperty("name").GetString();
            var position = emp.GetProperty("position").GetString();
            var tel = emp.GetProperty("tel").GetString();
            var employerRepository = new EmployerRepository();
            var success = employerRepository.СhangeEmployer(id, name, position, tel);
            if (success > 0)
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
        public dynamic Employer(int id)
        {
            var employerRepository = new EmployerRepository();
            var objCounts = employerRepository.DeleteItem(id);
            var responceObj = new
            {
                success = objCounts
            };
            return JsonSerializer.Serialize(responceObj);
        }

        [HttpGet]
        public dynamic GetEmployer()
        {
            var employerRepository = new EmployerRepository();
            var employerList = employerRepository.GetItems();
            if (employerList.Count > 0)
            {
                var responceObj = new
                {
                    success = 1,
                    employers = employerList
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
        public dynamic GetEmployer(int id)
        {
            var employerRepository = new EmployerRepository();
            var employer = employerRepository.GetItem(id);
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
}
