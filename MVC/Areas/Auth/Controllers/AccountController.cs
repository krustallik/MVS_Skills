using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Auth.Models.Forms;
using MVC.Models;
using System.Security.Claims;

namespace MVC.Areas.Auth.Controllers;
[Area("Auth")]
public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginForm());
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginForm form, string returnUrl)
    {
        // Якщо returnUrl порожній, встановлюємо значення за замовчуванням
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var user = await userManager.FindByEmailAsync(form.Login);
        if (user == null || !(await userManager.CheckPasswordAsync(user, form.Password)))
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(form);
        }

        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        
        return Redirect(returnUrl);
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterForm());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterForm form, string returnUrl)
    {
        // Якщо returnUrl порожній, встановлюємо значення за замовчуванням
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        if (await userManager.FindByEmailAsync(form.Login) != null)
        {
            ModelState.AddModelError(nameof(form.Login), "User already exists");
            return View(form);
        }

        var user = new User()
        {
            Email = form.Login,
            UserName = form.Login,
        };

        var result = await userManager.CreateAsync(user, form.Password);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(form.Login), "Error ...");
            return View(form);
        }

        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        return Redirect(returnUrl);
    }
}
