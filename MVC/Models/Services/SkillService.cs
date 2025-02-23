using MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models.Services
{
    public class SkillService
    {
        private readonly SiteContext _context;
        public SkillService(SiteContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>> GetAllAsync(int userId)
        {
            return await _context.Skills.Where(s => s.UserInfoId == userId).ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(int userId, int id)
        {
            return await _context.Skills.FirstOrDefaultAsync(s => s.UserInfoId == userId && s.Id == id);
        }

        public async Task AddAsync(int userId, Skill skill, IFormFile? file = null)
        {
            // Призначаємо користувачу
            skill.UserInfoId = userId;

            // Обробка логотипу, якщо потрібно
            if (file != null && file.Length > 0)
            {
                skill.LogoPath = SaveImage(file, userId, skill.Id);
            }
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int userId, Skill skill, IFormFile? file = null)
        {
            var existing = await GetByIdAsync(userId, skill.Id);
            if (existing != null)
            {
                existing.Title = skill.Title;
                existing.Color = skill.Color;
                existing.Level = skill.Level;

                if (file != null && file.Length > 0)
                {
                    DeleteImage(existing.LogoPath);
                    existing.LogoPath = SaveImage(file, userId, skill.Id);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int userId, int id)
        {
            var skill = await GetByIdAsync(userId, id);
            if (skill != null)
            {
                DeleteImage(skill.LogoPath);
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
        }

        // Методи SaveImage та DeleteImage можуть залишитись подібними до існуючих, оскільки вони відповідають за збереження файлів у wwwroot
        private string SaveImage(IFormFile file, int userId, int skillId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "skills");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"user_{userId}_skill_{skillId}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/skills/{uniqueFileName}";
        }

        private void DeleteImage(string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}
