using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models.Services
{
    public class HomeService
    {
        private const int MaxResults = 24;
        private readonly SiteContext _context;

        public HomeService(SiteContext context)
        {
            _context = context;
        }

        public async Task<List<SearchProfileViewModel>> SearchAsync(SearchDto criteria)
        {
            IQueryable<UserInfo> query = _context.UserInfos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(criteria.SearchText))
            {
                var term = criteria.SearchText.Trim()
                    .Replace("\\", "\\\\")
                    .Replace("%", "\\%")
                    .Replace("_", "\\_");
                query = query.Where(user => EF.Functions.Like(user.Name, $"%{term}%", "\\"));
            }

            if (criteria.SelectedProfessions is { Count: > 0 })
            {
                query = query.Where(user => user.Profession != null
                    && criteria.SelectedProfessions.Contains(user.Profession));
            }

            if (criteria.SelectedSkills is { Count: > 0 })
            {
                query = query.Where(user => user.UserSkills
                    .Any(userSkill => criteria.SelectedSkills.Contains(userSkill.SkillId)));
            }

            return await query
                .OrderBy(user => user.Name)
                .ThenBy(user => user.Id)
                .Take(MaxResults)
                .Select(user => new SearchProfileViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Profession = user.Profession,
                    ExperienceYears = user.ExpirienseYears,
                    Skills = user.UserSkills
                        .OrderBy(userSkill => userSkill.Skill.Title)
                        .Take(3)
                        .Select(userSkill => userSkill.Skill.Title)
                        .ToList(),
                    SkillCount = user.UserSkills.Count,
                    AverageRating = user.UserRatings.Select(rating => (double?)rating.Rating).Average(),
                    RatingCount = user.UserRatings.Count
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAllProfessionsAsync()
        {
            return await _context.UserInfos
                .AsNoTracking()
                .Where(user => user.Profession != null)
                .Select(user => user.Profession!)
                .Distinct()
                .OrderBy(profession => profession)
                .Select(profession => new SelectListItem { Value = profession, Text = profession })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .AsNoTracking()
                .OrderBy(skill => skill.Title)
                .Select(skill => new SelectListItem
                {
                    Value = skill.Id.ToString(),
                    Text = skill.Title
                })
                .ToListAsync();
        }
    }
}