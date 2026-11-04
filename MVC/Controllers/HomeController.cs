using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Services;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private const int MaxSearchTextLength = 100;
        private const int MaxSelectedFilters = 20;
        private readonly HomeService _homeService;

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Professions = await _homeService.GetAllProfessionsAsync();
            ViewBag.Skills = await _homeService.GetAllSkillsAsync();
            return View(await _homeService.SearchAsync(new SearchDto()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromBody] SearchDto? model)
        {
            if (model is null
                || model.SearchText?.Length > MaxSearchTextLength
                || model.SelectedProfessions?.Count > MaxSelectedFilters
                || model.SelectedProfessions?.Any(profession => string.IsNullOrWhiteSpace(profession)
                    || profession.Length > MaxSearchTextLength) == true
                || model.SelectedSkills?.Count > MaxSelectedFilters
                || model.SelectedSkills?.Any(skillId => skillId <= 0) == true)
            {
                return BadRequest("Search filters are invalid.");
            }

            model.SearchText = model.SearchText?.Trim();
            return PartialView("_UserList", await _homeService.SearchAsync(model));
        }
    }
}