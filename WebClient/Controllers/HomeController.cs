using System.Text.Json;
using DB.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Logger;
using WebClient.Models;

namespace WebClient.Controllers;

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
        //var qq = new ResultModelBuilder()
        //    .WithSuccess(true)
        //    .WithComputer(new Computer { ID = 1, Cpu = "Amd" })
        //    .Build();
        //return JsonSerializer.Serialize(qq);

        _logger.LogError(new Exception("123kkek"), "message");
        return "13";
    }
}