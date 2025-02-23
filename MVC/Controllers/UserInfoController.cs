using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Forms;
using MVC.Models.Services;

namespace MVC.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly ILogger<UserInfoController> _logger;
        private readonly UserInfoService _userService;
        private readonly SkillService _skillService;
        private readonly IWebHostEnvironment _env;

        public UserInfoController(
            ILogger<UserInfoController> logger,
            UserInfoService userService,
            SkillService skillService,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _userService = userService;
            _skillService = skillService;
            _env = env;
        }

        // Перегляд списку користувачів
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        // Перегляд інформації про користувача із завантаженням його навичок
        public async Task<IActionResult> View(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            // Якщо навички не завантажуються автоматично через Include, можна додатково їх отримати:
            // user.Skills = await _skillService.GetAllAsync(user.Id);
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserInfoForm(new UserInfo());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var model = new UserInfo();
            form.Update(model);

            // В EF Core Id генерується автоматично, тому не потрібно призначати його вручну

            // Обробка завантаження фотографій
            if (form.Photos != null && form.Photos.Any())
            {
                foreach (var photo in form.Photos)
                {
                    var photoPath = SaveUserPhoto(photo);
                    model.PhotoPaths.Add(photoPath);
                }
                if (model.PhotoPaths.Any())
                {
                    model.AvatarPhoto = model.PhotoPaths.First();
                }
            }
            else
            {
                model.PhotoPaths = new List<string>();
            }

            await _userService.AddAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(new UserInfoForm(user));
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(int id, [FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", form);
            }

            var model = await _userService.FindByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            form.Update(model);

            if (form.Photos != null && form.Photos.Any())
            {
                foreach (var photo in form.Photos)
                {
                    var photoPath = SaveUserPhoto(photo);
                    model.PhotoPaths.Add(photoPath);
                }
                if (string.IsNullOrEmpty(model.AvatarPhoto) && model.PhotoPaths.Any())
                {
                    model.AvatarPhoto = model.PhotoPaths.First();
                }
            }

            await _userService.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private string SaveUserPhoto(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "users");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"user_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/users/{uniqueFileName}";
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteByIdAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SetAvatar(int id, string photo)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user != null && user.PhotoPaths.Contains(photo))
            {
                user.AvatarPhoto = photo;
                await _userService.SaveChangesAsync();
            }
            return RedirectToAction("View", new { id });
        }
    }
}
