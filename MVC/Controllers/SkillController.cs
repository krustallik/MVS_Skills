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

        public SkillController(IWebHostEnvironment env)
        {
            _service = new SkillService(env);
        }

        // Перегляд списку навичок для конкретного користувача (userId передається як параметр)
        public IActionResult Index(int userId)
        {
            var skills = _service.GetAll(userId);
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
        public IActionResult Create(Skill model, int userId, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _service.Add(userId, model, file);
                return RedirectToAction("Index", new { userId });
            }

            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(model);
        }



        public IActionResult Edit(int id, int userId)
        {
            var skill = _service.GetById(userId, id);
            if (skill == null)
            {
                return NotFound();
            }
            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(skill);
        }

        [HttpPost]
        public IActionResult Edit(Skill model, int userId, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _service.Update(userId, model, file);
                return RedirectToAction("Index", new { userId });
            }

            ViewBag.UserId = userId;
            ViewBag.ColorOptions = _colorOptions;
            return View(model);
        }

        public IActionResult Delete(int id, int userId)
        {
            var skill = _service.GetById(userId, id);
            if (skill == null)
            {
                return NotFound();
            }
            ViewBag.UserId = userId;
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id, int userId)
        {
            _service.Delete(userId, id);
            return RedirectToAction("Index", new { userId = userId });
        }

    }
}
