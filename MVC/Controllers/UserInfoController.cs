using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Forms;
using MVC.Models.Services;
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
        public IActionResult Index()
        {
            return View(_userService.GetAll());
        }

        // Перегляд інформації про користувача із завантаженням його навичок
        public IActionResult View(int id)
        {
            var user = _userService.FindById(id);
            if (user != null)
            {
                // Завантажуємо навички для цього користувача із файлу skills_{user.Id}.json
                user.Skills = _skillService.GetAll(user.Id);
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserInfoForm(new UserInfo());
            return View(model);
        }

        [HttpPost]
        public IActionResult Create([FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var model = new UserInfo();
            form.Update(model);

            // Призначення випадкового Id, яке не повторюється
            var random = new Random();
            do
            {
                var id = random.Next(1, 10000);
                if (_userService.FindById(id) != null)
                {
                    continue;
                }
                model.Id = id;
            } while (model.Id == 0);

            // Обробка завантаження фотографій
            if (form.Photos != null && form.Photos.Any())
            {
                foreach (var photo in form.Photos)
                {
                    var photoPath = SaveUserPhoto(photo, model.Id);
                    model.PhotoPaths.Add(photoPath);
                }
                // Якщо аватар ще не задано, вибираємо перше фото як аватар
                if (model.PhotoPaths.Any())
                {
                    model.AvatarPhoto = model.PhotoPaths.First();
                }
            }
            else
            {
                // Ініціалізуємо порожній список, якщо фото не завантажено
                model.PhotoPaths = new List<string>();
            }

            // Ініціалізуємо порожній список навичок для нового користувача
            _skillService.SaveAll(model.Id, new List<Skill>());

            _userService.Add(model);
            _userService.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["id"] = id;
            return View(new UserInfoForm(_userService.FindById(id)));
        }

        [HttpPost]
        public IActionResult EditPost(int id, [FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid)
            {
                ViewData["id"] = id;
                return View("Edit", form);
            }

            var model = _userService.FindById(id);
            if (model == null)
            {
                return NotFound();
            }

            // Оновлюємо дані користувача з форми
            form.Update(model);

            // Обробка нових фотографій (якщо завантажено)
            if (form.Photos != null && form.Photos.Any())
            {
                foreach (var photo in form.Photos)
                {
                    var photoPath = SaveUserPhoto(photo, model.Id);
                    model.PhotoPaths.Add(photoPath);
                }
                if (string.IsNullOrEmpty(model.AvatarPhoto) && model.PhotoPaths.Any())
                {
                    model.AvatarPhoto = model.PhotoPaths.First();
                }
            }

            _userService.SaveChanges();
            return RedirectToAction("Index");
        }

        private string SaveUserPhoto(IFormFile file, int userId)
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

            string uniqueFileName = $"user_{userId}_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/users/{uniqueFileName}";
        }

        public IActionResult Delete(int id)
        {
            var user = _userService.FindById(id);
            if (user != null)
            {
                _userService.DeleteById(id);
                _userService.SaveChanges();
                // При бажанні можна видалити і файл з навичками для цього користувача
                // (реалізація видалення файлу може бути додана до SkillService)
            }
            return RedirectToAction("Index");
        }

        public IActionResult SetAvatar(int id, string photo)
        {
            var user = _userService.FindById(id);
            if (user != null && user.PhotoPaths.Contains(photo))
            {
                user.AvatarPhoto = photo;
                _userService.SaveChanges();
            }
            return RedirectToAction("View", new { id });
        }


    }
}
