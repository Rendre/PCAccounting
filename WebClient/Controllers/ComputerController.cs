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

        [HttpGet]
        public dynamic ComputerTest()
        {
            var id = 15;
            var name = "TestComp";
            var statusId = 1;
            var employerId = 1;
            var date = DateTime.UtcNow;
            var cpu = "pentium";
            decimal price = 322;

            var newId = _computerRepository.CreateComputer(name, statusId, employerId, date, cpu, price);

            var computer = _computerRepository.GetComputer(newId);

            var success = _computerRepository.ChangeComputer(newId, "AMD", 2, 2, DateTime.UtcNow, "AMD", 1488);

            return 1;
        }
    }
}
