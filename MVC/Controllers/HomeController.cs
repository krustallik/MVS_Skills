using MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MVC.Controllers;

public class HomeController(
    ILogger<HomeController> logger
    ) : Controller
{

    //     /Home/Index
    public IActionResult Index()
    {
        return View();
    }

    //     /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    //     /Home/Hello
    public IActionResult Hello()
    {
        List<Skill> list = new List<Skill>
    {
        //new Skill("C", 50),
        //new Skill("C++", 50),
        //new Skill("C#", 50),
        //new Skill("Entity Framework", 40),
        //new Skill("Java", 60),
        //new Skill("JDBC", 35),
        //new Skill("JPA", 35),
        //new Skill("HTML", 70),
        //new Skill("CSS", 60),
        //new Skill("JavaScript", 40),
        //new Skill("Python", 50),
        //new Skill("MicroPython (Raspberry Pi)", 40),
        //new Skill("Linux OS", 70),
        //new Skill("CISCO (Computer Networks)", 50),
        //new Skill("MATLAB", 35),
        //new Skill("PostgreSQL", 50),
        //new Skill("Microsoft SQL", 50)
    };

        // Створюємо об'єкт UserInfo
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
            Skills = list
        };

        return View(user); // Передаємо модель UserInfo у View
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
