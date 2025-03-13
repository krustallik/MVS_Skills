using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models.Services
{
    public class HomeService
    {
        private readonly SiteContext _context;

        public HomeService(SiteContext context)
        {
            _context = context;
        }

        // Отримати всіх користувачів разом з їхніми навичками
        public async Task<List<UserInfo>> GetAllUsersAsync()
        {
            return await _context.UserInfos
                                 .Include(u => u.UserSkills)
                                 .ThenInclude(us => us.Skill)
                                 .Include(u => u.Owner)
                                 .Include(u => u.UserRatings)
                                 .ToListAsync();
        }

        // Отримати всі професії
        public async Task<List<SelectListItem>> GetAllProfessionsAsync()
        {
            return await _context.UserInfos
                .Where(u => u.Profession != null)
                .Select(u => u.Profession)
                .Distinct()
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToListAsync();
        }

        // Отримати всі навички
        public async Task<List<SelectListItem>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Title })
                .ToListAsync();
        }
    }
}
