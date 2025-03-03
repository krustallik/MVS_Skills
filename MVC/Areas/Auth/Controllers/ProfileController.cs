using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Areas.Auth.Models.Forms;
using MVC.Models.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MVC.Areas.Auth.Models.Services;

namespace MVC.Areas.Auth.Controllers
{
    [Area("Auth")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SiteContext _context;
        private readonly ProfileService _profileService;

        public ProfileController(UserManager<User> userManager, SiteContext context, ProfileService profileService)
        {
            _userManager = userManager;
            _context = context;
            _profileService = profileService;
        }

        // Сторінка профіля
        public async Task<IActionResult> Index()
        {
            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstAsync(x => x.Id == id);
            return View(user);
        }

        // GET: Редагування профіля
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            var model = new ProfileForm
            {
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                ImagePath = user.ImagePath
            };
            return View(model);
        }

        // POST: Редагування профіля
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileForm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            await _profileService.UpdateProfileAsync(user, model.Image, model.FullName, model.Phone);
            return RedirectToAction("Index");
        }

        // GET: Зміна паролю
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Зміна паролю
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordForm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Новий пароль та підтвердження не співпадають.");
                return View(model);
            }

            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: Видалення акаунту (підтвердження)
        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        // POST: Видалення акаунту
        [HttpPost, ActionName("DeleteAccount")]
        public async Task<IActionResult> DeleteAccountConfirmed()
        {
            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                // Після видалення виходимо з системи
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
    }
}
