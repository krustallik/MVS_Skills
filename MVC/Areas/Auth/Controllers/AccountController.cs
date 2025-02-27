using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Auth.Controllers;
[Area("Auth")]
public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
}
