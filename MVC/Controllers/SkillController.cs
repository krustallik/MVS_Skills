using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Models;
using MVC.Models.Services;

namespace MVC.Controllers
{
    public class SkillController : Controller
    {
        private readonly SkillService _service;
        private readonly List<SelectListItem> _colorOptions = new()
        {
            new() { Value = "#FF5733", Text = "Coral" },
            new() { Value = "#33FF57", Text = "Lime Green" },
            new() { Value = "#3357FF", Text = "Royal Blue" },
            new() { Value = "#FF33A8", Text = "Hot Pink" }
        };

        public SkillController(SkillService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int userId)
        {
            var skills = await _service.GetAllAsync(userId);
            ViewBag.UserId = userId;
            return View(skills);
        }

        public IActionResult Create(int userId)
        {
            // Ініціалізуємо нову сутність UserSkill із новим об'єктом Skill
            var newUserSkill = new UserSkill { Skill = new Skill() };
            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(newUserSkill);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, string title, string color, int level, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                ViewBag.ColorOptions = _colorOptions;
                return View();
            }

            var skill = new Skill
            {
                Title = title,
                Color = color
            };

            await _service.AddAsync(userId, skill, level, file);

            return RedirectToAction("Index", new { userId });
        }




        public async Task<IActionResult> Edit(int id, int userId)
        {
            var userSkill = await _service.GetByIdAsync(userId, id);
            if (userSkill == null) return NotFound();
            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(userSkill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int userId, string title, string color, int level, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                ViewBag.ColorOptions = _colorOptions;
                return View();
            }

            await _service.UpdateAsync(userId, id, title, color, level, file);
            return RedirectToAction("Index", new { userId });
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int skillId, int userId)
        {
            var userSkill = await _service.GetByIdAsync(userId, skillId);
            if (userSkill == null) return NotFound();

            ViewBag.UserId = userId;
            return View(userSkill);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int skillId, int userId)
        {
            await _service.RemoveSkillFromUserAsync(userId, skillId);
            return RedirectToAction("Index", new { userId });
        }

    }
}
