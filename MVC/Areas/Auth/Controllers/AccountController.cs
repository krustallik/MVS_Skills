using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Auth.Models.Forms;
using MVC.Models;

namespace MVC.Areas.Auth.Controllers;

[Area("Auth")]
public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : Controller
{
    [HttpGet]
    public IActionResult Login() => View(new LoginForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginForm form, string? returnUrl)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var user = await userManager.FindByEmailAsync(form.Login);
        if (user == null || !(await signInManager.PasswordSignInAsync(
                user,
                form.Password,
                isPersistent: false,
                lockoutOnFailure: true)).Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(form);
        }

        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterForm());

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] RegisterForm form, string? returnUrl)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var existingUser = await userManager.FindByEmailAsync(form.Login);
        if (existingUser != null)
        {
            return RegistrationResponse(returnUrl);
        }

        var user = new User
        {
            Email = form.Login,
            UserName = form.Login,
        };

        var result = await userManager.CreateAsync(user, form.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return RegistrationResponse(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(form);
        }

        var roleResult = await userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return RegistrationResponse(returnUrl);
        }

        return RegistrationResponse(returnUrl);
    }

    private IActionResult RegistrationResponse(string? returnUrl)
    {
        TempData["AccountNotice"] = "If registration can be completed, your account is ready. Sign in to continue.";
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}