using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Models;
using MVC.Models.Services;
using System.Runtime.CompilerServices;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillController : Controller
    {
        // Поле для доступу до сервісу навичок
        private readonly SkillService _service;

        // Список кольорових опцій для вибору в інтерфейсі
        private readonly List<SelectListItem> _colorOptions = new()
        {
            new() { Value = "#A9531E", Text = "Burnt orange" },
            new() { Value = "#D5A03B", Text = "Honey" },
            new() { Value = "#647486", Text = "Slate blue" },
            new() { Value = "#765A48", Text = "Walnut" }
        };

        // Конструктор контролера, ініціалізує сервіс
        public SkillController(SkillService service)
        {
            _service = service;
        }

        // Метод для відображення списку навичок користувача
        public async Task<IActionResult> Index(int userId)
        {
            var skills = await _service.GetAllAsync(userId); // Отримання навичок з сервісу
            ViewBag.UserId = userId; // Передача userId у ViewBag
            return View(skills); // Повернення представлення з даними
        }

        // Метод для відображення сторінки створення нової навички
        public async Task<IActionResult> Create(int userId)
        {
            var newUserSkill = new UserSkill { Skill = new Skill() }; // Створюємо нову навичку
            ViewBag.UserId = userId; // Передача userId у ViewBag
            ViewBag.ColorOptions = _colorOptions; // Передача опцій кольору
            List<SelectListItem> _skillOptions = await _service.GetAllAsync(); // Очікуємо асинхронний виклик
            ViewBag.SkillOptions = _skillOptions;
            return View(newUserSkill); // Повертаємо представлення (без await)
        }


        // Метод для обробки POST-запиту на створення навички
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(6_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 6_000_000)]
        public async Task<IActionResult> Create(int userId, string title, string color, int level, IFormFile? file)
        {
            if (file != null && file.Length > SkillService.MaxLogoBytes)
            {
                return BadRequest("Skill logos must be 5 MB or smaller.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                ViewBag.ColorOptions = _colorOptions;
                ViewBag.SkillOptions = await _service.GetAllAsync();
                ViewBag.SelectedTitle = title;
                ViewBag.SelectedColor = color;
                ViewBag.SubmittedLevel = Request.Form["level"].ToString();
                return View(new UserSkill { UserInfoId = userId, Level = level, Skill = new Skill { Title = title, Color = color } });
            }

            var skill = new Skill
            {
                Title = title,
                Color = color
            };

            await _service.AddAsync(userId, skill, level, file); // Додаємо навичку через сервіс
            return RedirectToAction("Index", new { userId }); // Перенаправлення на список навичок
        }

        // Метод для відображення сторінки редагування навички
        public async Task<IActionResult> Edit(int id, int userId)
        {
            var userSkill = await _service.GetByIdAsync(userId, id); // Отримання навички з сервісу
            if (userSkill == null) return NotFound(); // Якщо навичка не знайдена - повертаємо 404

            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;

            List<SelectListItem> _skillOptions = await _service.GetAllAsync(); // Очікуємо асинхронний виклик
            ViewBag.SkillOptions = _skillOptions;

            return View(userSkill); // Повернення представлення з даними
        }

        // Метод для обробки POST-запиту на оновлення навички
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(6_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 6_000_000)]
        public async Task<IActionResult> Edit(int id, int userId, string title, string color, int level, IFormFile? file)
        {
            if (file != null && file.Length > SkillService.MaxLogoBytes)
            {
                return BadRequest("Skill logos must be 5 MB or smaller.");
            }

            if (!ModelState.IsValid)
            {
                var userSkill = await _service.GetByIdAsync(userId, id);
                if (userSkill == null) return NotFound();

                ViewBag.UserId = userId;
                ViewBag.ColorOptions = _colorOptions;
                ViewBag.SkillOptions = await _service.GetAllAsync();
                ViewBag.SelectedTitle = title;
                ViewBag.SelectedColor = color;
                ViewBag.SubmittedLevel = Request.Form["level"].ToString();
                return View(userSkill);
            }

            await _service.UpdateAsync(userId, id, title, color, level, file); // Оновлення навички
            return RedirectToAction("Index", new { userId }); // Перенаправлення на список навичок
        }

        // Метод для відображення сторінки видалення навички
        [HttpGet]
        public async Task<IActionResult> Delete(int skillId, int userId)
        {
            var userSkill = await _service.GetByIdAsync(userId, skillId); // Отримання навички
            if (userSkill == null) return NotFound(); // Якщо навичка не знайдена - повертаємо 404

            ViewBag.UserId = userId;
            return View(userSkill); // Повернення представлення
        }

        // Метод для підтвердження видалення навички
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int skillId, int userId)
        {
            await _service.RemoveSkillFromUserAsync(userId, skillId); // Видалення навички через сервіс
            return RedirectToAction("Index", new { userId }); // Перенаправлення на список навичок
        }
    }
}
