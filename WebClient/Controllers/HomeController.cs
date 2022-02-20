using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public string Index()
        {
            return "123";
        }
    }
}