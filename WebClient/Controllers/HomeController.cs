namespace WebClient.Controllers;
using DB;
using Microsoft.AspNetCore.Mvc;
using DB.Repositories.Task;

[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public string Index()
    {
        var taskRepository = new TaskDapperRepository();
        var qq = taskRepository.GetItems(type: TaskType.Auth, name:"FirstTask");
        return "123";
    }
}