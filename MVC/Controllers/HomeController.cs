// HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Skills()
        {
            List<Skill> list = new List<Skill>
            {
                new Skill("C", 50),
                new Skill("C++", 50),
                new Skill("C#", 50),
                new Skill("Entity Framework", 40),
                new Skill("Java", 60),
                new Skill("JDBC", 35),
                new Skill("JPA", 35),
                new Skill("HTML", 70),
                new Skill("CSS", 60),
                new Skill("JavaScript", 40),
                new Skill("Python", 50),
                new Skill("MicroPython (Raspberry Pi)", 40),
                new Skill("Linux OS", 70),
                new Skill("CISCO (Computer Networks)", 50),
                new Skill("MATLAB", 35),
                new Skill("PostgreSQL", 50),
                new Skill("Microsoft SQL", 50)
            };

            ViewData["List"] = list;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}