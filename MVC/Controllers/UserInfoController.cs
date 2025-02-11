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

        public UserInfoController(
            ILogger<UserInfoController> logger,
            UserInfoService userService,
            SkillService skillService)
        {
            _logger = logger;
            _userService = userService;
            _skillService = skillService;
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
        public IActionResult Edit(int id, [FromForm] UserInfoForm form)
        {
            if (!ModelState.IsValid)
            {
                ViewData["id"] = id;
                return View(form);
            }

            var model = _userService.FindById(id);
            form.Update(model);
            _userService.SaveChanges();
            return RedirectToAction("Index");
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
    }
}
