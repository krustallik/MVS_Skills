using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models.Services
{
    public class SkillService
    {
        private readonly SiteContext _context;

        // Конструктор сервісу, ініціалізує контекст бази даних
        public SkillService(SiteContext context)
        {
            _context = context;
        }

        // Отримання всіх навичок користувача
        public async Task<List<UserSkill>> GetAllAsync(int userId)
        {
            return await _context.UserSkills
                                 .Where(us => us.UserInfoId == userId)
                                 .Include(us => us.Skill)
                                 .ToListAsync();
        }
        //Отримання всіх існуючих навиків з датабази
        public async Task<List<SelectListItem>> GetAllAsync()
        {
            return await _context.Skills
                .Select(s => new SelectListItem
                {
                    Value = s.Title,
                    Text = s.Title
                })
                .ToListAsync();
        }


        // Отримання конкретної навички користувача за ID
        public async Task<UserSkill?> GetByIdAsync(int userId, int skillId)
        {
            return await _context.UserSkills
                                 .Include(us => us.Skill)
                                 .FirstOrDefaultAsync(us => us.UserInfoId == userId && us.SkillId == skillId);
        }

        // Додавання нової навички користувачу
        public async Task AddAsync(int userId, Skill skill, int level, IFormFile? file = null)
        {
            var existingSkill = await _context.Skills.FirstOrDefaultAsync(s => s.Title == skill.Title);

            if (existingSkill == null)
            {
                if (file != null && file.Length > 0)
                {
                    skill.LogoPath = SaveImage(file, userId);
                }
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();
                existingSkill = skill;
            }

            var userSkillInDatabase = await _context.UserSkills.FirstOrDefaultAsync(us => us.UserInfoId == userId && us.SkillId == existingSkill.Id);

            if (userSkillInDatabase == null)
            {
                // Створюємо новий зв'язок
                var userSkill = new UserSkill
                {
                    UserInfoId = userId,
                    SkillId = existingSkill.Id,
                    Level = level
                };
                _context.UserSkills.Add(userSkill);
            }
            else
            {
                // Оновлюємо дані навички
                //TODO Адмін міняти може все а користувач лише рівень навички
                userSkillInDatabase.Level = level;
                userSkillInDatabase.Skill.Color = skill.Color;
                userSkillInDatabase.Skill.LogoPath = SaveImage(file,userId);
                _context.UserSkills.Update(userSkillInDatabase);
            }

            await _context.SaveChangesAsync();
        }


        // Оновлення інформації про навичку
        public async Task UpdateAsync(int userId, int skillId, string newTitle, string newColor, int newLevel, IFormFile? file = null)
        {
            var userSkill = await GetByIdAsync(userId, skillId);
            if (userSkill != null)
            {
                userSkill.Level = newLevel;
                userSkill.Skill.Title = newTitle;
                userSkill.Skill.Color = newColor;

                if (file != null && file.Length > 0)
                {
                    if (!string.IsNullOrEmpty(userSkill.Skill.LogoPath))
                    {
                        DeleteImage(userSkill.Skill.LogoPath);
                    }
                    userSkill.Skill.LogoPath = SaveImage(file, userId);
                }

                await _context.SaveChangesAsync();
            }
        }

        // Видалення навички у конкретного користувача
        public async Task RemoveSkillFromUserAsync(int userId, int skillId)
        {
            var userSkill = await _context.UserSkills
                                          .FirstOrDefaultAsync(us => us.UserInfoId == userId && us.SkillId == skillId);
            if (userSkill != null)
            {
                _context.UserSkills.Remove(userSkill);
                await _context.SaveChangesAsync();
            }
        }

        // Видалення навички з бази даних
        public async Task DeleteAsync(int skillId)
        {
            var skill = await _context.Skills.Include(s => s.UserSkills)
                                             .FirstOrDefaultAsync(s => s.Id == skillId);
            if (skill != null)
            {
                _context.UserSkills.RemoveRange(skill.UserSkills);
                if (!string.IsNullOrEmpty(skill.LogoPath))
                {
                    DeleteImage(skill.LogoPath);
                }
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
        }

        // Збереження зображення навички та повернення його шляху
        private string SaveImage(IFormFile file, int userId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "skills");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"user_{userId}_skill_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/skills/{uniqueFileName}";
        }

        // Видалення зображення з файлової системи
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
