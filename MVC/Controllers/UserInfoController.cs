﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.Forms;
using MVC.Models.Services;
using System.Security.Claims;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserInfoController : Controller
    {
        // Логер для запису інформації про роботу контролера
        private readonly ILogger<UserInfoController> _logger;

        // Сервіс для роботи з інформацією про користувачів
        private readonly UserInfoService _userService;

        // Сервіс для роботи з навичками користувачів
        private readonly SkillService _skillService;

        // Оточення хоста для роботи з файлами
        private readonly IWebHostEnvironment _env;
        private readonly SiteContext _siteContext;

        // Конструктор контролера, ініціалізує залежності
        public UserInfoController(
            ILogger<UserInfoController> logger,
            UserInfoService userService,
            SkillService skillService,
            IWebHostEnvironment env,
            SiteContext context)
        {
            _logger = logger;
            _userService = userService;
            _skillService = skillService;
            _env = env;
            _siteContext = context;
        }

        // Відображення списку всіх користувачів
        public async Task<IActionResult> Index()
        {
            List<UserInfo> userInfos;
            if (User.IsInRole("Admin"))
            {
                // Адміністратор бачить усі записи
                userInfos = await _siteContext.UserInfos
                                          .Include(ui => ui.Owner)
                                          .Include(ui => ui.UserRatings)
                                          .ToListAsync();
            }
            else
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                // Звичайний користувач бачить лише свої записи
                userInfos = await _siteContext.UserInfos
                                          .Where(ui => ui.OwnerId == currentUserId)
                                          .ToListAsync();
            }

            return View(userInfos);
        }


        // Перегляд інформації про конкретного користувача
        public async Task<IActionResult> View(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpGet]
        // Відображення форми для створення нового користувача
        public IActionResult Create() => View(new UserInfoForm(new UserInfo()));

        [HttpPost]
        // Обробка запиту на створення нового користувача
        public async Task<IActionResult> Create([FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid) return View(form);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var model = new UserInfo();
            form.Update(model);

            // Обробка завантаження фото користувача
            model.PhotoPaths = ProcessUploadedPhotos(form.Photos);
            model.AvatarPhoto = model.PhotoPaths.FirstOrDefault();
            model.OwnerId = currentUserId;

            await _userService.AddAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        // Відображення форми для редагування користувача
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            return user == null ? NotFound() : View(new UserInfoForm(user));
        }

        [HttpPost]
        // Обробка запиту на редагування користувача
        public async Task<IActionResult> EditPost([FromForm] UserInfoForm form)
        {
            Console.WriteLine($"ID з форми: {form.Id}");

            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"Поле: {modelError.Key} - Помилка: {error.ErrorMessage}");
                    }
                }
                return View("Edit", form);
            }

            var model = await _userService.FindByIdAsync(form.Id);
            if (model == null) return NotFound();

            form.Update(model);

            // Видалення старих фото та додавання нових
            if (form.Photos != null && form.Photos.Any())
            {
                DeleteExistingPhotos(model);
                model.PhotoPaths = ProcessUploadedPhotos(form.Photos);
                model.AvatarPhoto = model.PhotoPaths.FirstOrDefault();
            }

            await _userService.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Відображення підтвердження видалення користувача
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost, ActionName("Delete")]
        // Підтвердження та видалення користувача
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null) return NotFound();

            DeleteExistingPhotos(user);
            await _userService.DeleteByIdAsync(id);

            return RedirectToAction("Index");
        }

        // Встановлення аватару для користувача
        public async Task<IActionResult> SetAvatar(int id, string photo)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null || !user.PhotoPaths.Contains(photo)) return NotFound();

            user.AvatarPhoto = photo;
            await _userService.SaveChangesAsync();
            return RedirectToAction("View", new { id });
        }

        // Збереження завантажених фото та повернення їхніх шляхів
        private List<string> ProcessUploadedPhotos(List<IFormFile>? photos)
        {
            var photoPaths = new List<string>();
            if (photos == null || !photos.Any()) return photoPaths;

            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "users");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var photo in photos)
            {
                var fileExtension = Path.GetExtension(photo.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
                }

                string uniqueFileName = $"user_{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                photo.CopyTo(fileStream);

                photoPaths.Add($"/images/users/{uniqueFileName}");
            }

            return photoPaths;
        }

        // Видалення старих фото користувача
        private void DeleteExistingPhotos(UserInfo user)
        {
            foreach (var photoPath in user.PhotoPaths)
            {
                string fullPath = Path.Combine(_env.WebRootPath, photoPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            user.PhotoPaths.Clear();
            user.AvatarPhoto = null;
        }

        [HttpPost]
        public async Task<IActionResult> Rate(int userInfoId, int rating)
        {
            // Отримати поточного користувача з Claims
            int currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            // Завантажити об'єкт UserInfo з бази даних, включаючи оцінки (якщо потрібно)
            var userInfo = await _siteContext.UserInfos
                .Include(ui => ui.UserRatings)
                .FirstOrDefaultAsync(ui => ui.Id == userInfoId);

            if (userInfo == null)
            {
                return NotFound();
            }

            // Заборонити голосувати самому собі
            if (userInfo.OwnerId == currentUserId)
            {
                // Можна повернути повідомлення про помилку або редірект з повідомленням
                ModelState.AddModelError("", "Неможливо оцінити власний профіль.");
                return RedirectToAction("View", new { id = userInfoId });
            }

            // Перевірка чи користувач вже голосував
            if (userInfo.UserRatings.Any(r => r.RaterId == currentUserId))
            {
                ModelState.AddModelError("", "Ви вже голосували за цього користувача.");
                return RedirectToAction("View", new { id = userInfoId });
            }

            // Створюємо новий запис оцінки
            var userRating = new UserRating
            {
                UserInfoId = userInfoId,
                RaterId = currentUserId,
                Rating = rating
            };

            _siteContext.UserRatings.Add(userRating);
            await _siteContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }

    }
}
