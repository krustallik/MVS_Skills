using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Models;
using MVC.Models.Services;

namespace MVC.Controllers
{
    public class SkillController : Controller
    {
        private readonly SkillService _service;
        // 10 варіантів кольорів для вибору
        private readonly List<SelectListItem> _colorOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "#FF5733", Text = "Coral" },
            new SelectListItem { Value = "#33FF57", Text = "Lime Green" },
            new SelectListItem { Value = "#3357FF", Text = "Royal Blue" },
            new SelectListItem { Value = "#FF33A8", Text = "Hot Pink" },
            new SelectListItem { Value = "#33FFF0", Text = "Turquoise" },
            new SelectListItem { Value = "#A833FF", Text = "Purple" },
            new SelectListItem { Value = "#FF8F33", Text = "Orange" },
            new SelectListItem { Value = "#8FFF33", Text = "Light Green" },
            new SelectListItem { Value = "#338FFF", Text = "Azure" },
            new SelectListItem { Value = "#FF3333", Text = "Red" }
        };

        public SkillController(SkillService service)
        {
            _service = service;
        }

        // Перегляд списку навичок для конкретного користувача (userId передається як параметр)
        public async Task<IActionResult> Index(int userId)
        {
            var skills = await _service.GetAllAsync(userId);
            ViewBag.UserId = userId;
            return View(skills);
        }

        public IActionResult Create(int userId)
        {
            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Skill model, int userId, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(userId, model, file);
                return RedirectToAction("Index", new { userId });
            }

            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id, int userId)
        {
            var skill = await _service.GetByIdAsync(userId, id);
            if (skill == null)
            {
                return NotFound();
            }
            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(skill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Skill model, int userId, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(userId, model, file);
                return RedirectToAction("Index", new { userId });
            }

            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(model);
        }

        public async Task<IActionResult> Delete(int id, int userId)
        {
            var skill = await _service.GetByIdAsync(userId, id);
            if (skill == null)
            {
                return NotFound();
            }
            ViewBag.UserId = userId;
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int userId)
        {
            await _service.DeleteAsync(userId, id);
            return RedirectToAction("Index", new { userId });
        }
    }
}
