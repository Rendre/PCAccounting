using System.Text.Json;
using DB.Repositories.User;
using DB.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController()
        {
            _userRepository = new UserRepositoryDefault();
        }

        [HttpGet("{id:int}")]
        public dynamic GetUser(int id)
        {
            var user = _userRepository.GetItem(id);
            if (user != null)
            {
                var responseObj = new
                {
                    success = 1,
                    login = user.Login,
                    employerId = user.EmployerId
                };
                return responseObj;

            }
            else
            {
                var responseObj = new
                {
                    success = 0
                };
                return responseObj;

            }
        }

        [HttpGet]
        public dynamic GetUsers()
        {
            var userList = _userRepository.GetItems();
            var outUserList = new List<dynamic>();
            foreach (var user in userList)
            {
                var outUser = new
                {
                    id = user.Id,
                    login = user.Login,
                    employerId = user.EmployerId
                };
                outUserList.Add(outUser);
            }
            if (outUserList.Count > 0)
            {
                var responseObj = new
                {
                    success = 1,
                    users = outUserList
                };
                return responseObj;
            }
            else
            {
                var responseObj = new
                {
                    success = 0
                };
                return responseObj;
            }
        }

        [HttpDelete("{id:int}")]
        public dynamic DeleteUser(int id)
        {
            var deleteObjectCounts = _userRepository.DeleteItem(id);
            var resultObj = new
            {
                success = deleteObjectCounts
            };

            return resultObj;
        }

        [HttpPost]
        public dynamic CreateUser([FromBody] JsonElement userJsn)
        {
            var login = userJsn.GetProperty("login").GetString();
            var password = userJsn.GetProperty("password").GetString();
            password = Util.Encode(password);
            var employerId = userJsn.GetProperty("employerId").GetInt32();
            var userId = _userRepository.CreateUser(login, password, employerId);
            if (userId <= 0)
            {
                var resultObj = new
                {
                    success = 0
                };
                return resultObj;
            }
            else
            {
                var resultObj = new
                {
                    success = 1
                };
                return resultObj;
            }
        }

        [HttpPut]
        public dynamic ChangeUser([FromBody] JsonElement userJsn)
        {
            var id = userJsn.GetProperty("id").GetInt32();
            var login = userJsn.GetProperty("login").GetString();
            var password = userJsn.GetProperty("password").GetString();
            password = Util.Encode(password);
            var employerId = userJsn.GetProperty("employerId").GetInt32();
            var success = _userRepository.ChangeUser(id, login, password, employerId);
            return JsonSerializer.Serialize(success);
        }

    }
}
