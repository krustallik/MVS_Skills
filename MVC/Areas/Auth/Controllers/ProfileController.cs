using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using System.Security.Claims;

namespace MVC.Areas.Auth.Controllers;

[Area("Auth")]
[Authorize]
public class ProfileController(
    UserManager<User> userManager,
    SiteContext context
    ) : Controller
{
    public async Task<IActionResult> Index()
    {
        var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var user = await context.Users
            .FirstAsync(x => x.Id == id);
        return View(user);
    }
}
