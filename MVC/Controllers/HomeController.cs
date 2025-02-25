using MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // /Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // /Home/Hello
        public IActionResult Hello()
        {

            UserInfo user = new UserInfo
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Description = "Software Developer",
                Birthday = new DateTime(1990, 5, 15),
                IsActive = true,
                ExpirienseYears = 5,
                Salary = 70000,
            };

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
    