using System.Text.Json;
using DB.Repositories.Employer;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    [Route("[controller]")]
    public class EmployerController : ControllerBase
    {
        private readonly IEmployerRepository _employerRepository;

        public EmployerController()
        {
            _employerRepository = new EmployerRepositoryDapper();
        }

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
            var empId = _employerRepository.CreateEmployer(name, position, tel);
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
        
        [HttpPut]
        public dynamic PutEmployer([FromBody] JsonElement emp)
        {
            var id = emp.GetProperty("id").GetInt32();
            var name = emp.GetProperty("name").GetString();
            var position = emp.GetProperty("position").GetString();
            var tel = emp.GetProperty("tel").GetString();
            var success = _employerRepository.СhangeEmployer(id, name, position, tel);
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
            var deleteObjCounts = _employerRepository.DeleteItem(id);
            var responceObj = new
            {
                success = deleteObjCounts
            };
            return JsonSerializer.Serialize(responceObj);
        }

        [HttpGet]
        public dynamic GetEmployer()
        {
            var employerList = _employerRepository.GetItems();
            var outEmployerList = new List<dynamic>();
            foreach (var employer in employerList)
            {
                var outEmployer = new
                {
                    id = employer.Id,
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
        public dynamic GetEmployer(int id)
        {
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
}
