using Microsoft.AspNetCore.Http;
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

        public SkillService(SiteContext context)
        {
            _context = context;
        }

        // Отримати всі навички конкретного користувача
        public async Task<List<Skill>> GetAllAsync(int userId)
        {
            return await _context.UserSkills
                                 .Where(us => us.UserInfoId == userId)
                                 .Select(us => us.Skill)
                                 .ToListAsync();
        }

        // Отримати конкретну навичку користувача
        public async Task<Skill?> GetByIdAsync(int userId, int skillId)
        {
            return await _context.UserSkills
                                 .Where(us => us.UserInfoId == userId && us.SkillId == skillId)
                                 .Select(us => us.Skill)
                                 .FirstOrDefaultAsync();
        }

        // Додати нову навичку користувачеві
        public async Task AddAsync(int userId, Skill skill, IFormFile? file = null)
        {
            // Додаємо саму навичку, якщо її ще немає в БД
            var existingSkill = await _context.Skills
                                              .FirstOrDefaultAsync(s => s.Title == skill.Title);

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

            // Створюємо зв'язок між користувачем і навичкою
            var userSkill = new UserSkill
            {
                UserInfoId = userId,
                SkillId = existingSkill.Id
            };

            _context.UserSkills.Add(userSkill);
            await _context.SaveChangesAsync();
        }

        // Оновити навичку (дозволяється змінювати рівень, колір, лого)
        public async Task UpdateAsync(int userId, Skill updatedSkill, IFormFile? file = null)
        {
            var existingSkill = await GetByIdAsync(userId, updatedSkill.Id);
            if (existingSkill != null)
            {
                existingSkill.Title = updatedSkill.Title;
                existingSkill.Color = updatedSkill.Color;
                existingSkill.Level = updatedSkill.Level;

                if (file != null && file.Length > 0)
                {
                    if (existingSkill.LogoPath != null)
                    {
                        Directory.Delete(existingSkill.LogoPath);
                    }
                    existingSkill.LogoPath = SaveImage(file, userId);
                }

                _context.Skills.Update(existingSkill);
                await _context.SaveChangesAsync();
            }
        }

        // Видалити навичку у конкретного користувача (не видаляє саму навичку з БД)
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

        // Видалити навичку з БД (з усіх користувачів)
        public async Task DeleteAsync(int skillId)
        {
            var skill = await _context.Skills
                                      .Include(s => s.UserSkills)
                                      .FirstOrDefaultAsync(s => s.Id == skillId);

            if (skill != null)
            {
                // Видаляємо всі зв’язки
                _context.UserSkills.RemoveRange(skill.UserSkills);

                // Видаляємо саме вміння
                if (skill.LogoPath != null)
                {
                    Directory.Delete(skill.LogoPath);
                }
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
        }

        // Збереження зображення
        private string SaveImage(IFormFile file, int userId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "skills");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"user_{userId}_skill_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/images/skills/{uniqueFileName}";
        }
    }
}
