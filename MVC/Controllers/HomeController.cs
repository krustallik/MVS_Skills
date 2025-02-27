using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HomeService _homeService;

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Professions = await _homeService.GetAllProfessionsAsync();
            ViewBag.Skills = await _homeService.GetAllSkillsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromBody] SearchDto model)
        {
            var users = await _homeService.GetAllUsersAsync();

            // Фільтрація за ім'ям
            if (!string.IsNullOrEmpty(model.SearchText))
            {
                users = users.Where(u => u.Name.Contains(model.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Фільтрація за професіями
            if (model.SelectedProfessions != null && model.SelectedProfessions.Any())
            {
                users = users.Where(u => model.SelectedProfessions.Contains(u.Profession)).ToList();
            }

            // Фільтрація за навичками
            if (model.SelectedSkills != null && model.SelectedSkills.Any())
            {
                users = users.Where(u => u.UserSkills.Any(us => model.SelectedSkills.Contains(us.SkillId))).ToList();
            }

            return PartialView("_UserList", users);
        }

    }
}
