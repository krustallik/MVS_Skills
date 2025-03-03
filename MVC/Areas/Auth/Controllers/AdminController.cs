using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Auth.Models.Forms;
using MVC.Areas.Auth.Models.Services;
using MVC.Models;
using MVC.Models.Services;
using System.Threading.Tasks;

namespace MVC.Areas.Auth.Controllers;

[Area("Auth")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var (users, userRoles) = await _adminService.GetUsersWithRolesAsync();
        ViewBag.UserRoles = userRoles;
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _adminService.GetEditUserFormAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserForm editUserForm)
    {
        if (!ModelState.IsValid)
        {
            editUserForm.AvailableRoles = await _adminService.GetAvailableRolesAsync();
            return View(editUserForm);
        }

        var result = await _adminService.UpdateUserAsync(editUserForm);
        if (!result)
        {
            ModelState.AddModelError("", "Error updating user.");
            return View(editUserForm);
        }

        return RedirectToAction("Index");
    }
}
