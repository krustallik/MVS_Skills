using System.Text.Json;
using MVC.Models;

namespace MVC.Models.Services
{
    public class SkillService
    {
        private readonly IWebHostEnvironment _env;

        public SkillService(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Формує шлях до файлу для збереження навичок конкретного користувача.
        /// </summary>
        private string GetFilePath(int userId)
        {
            var folder = Path.Combine(_env.ContentRootPath, "App_Data");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, $"skills_{userId}.json");
        }

        public List<Skill> GetAll(int userId)
        {
            var filePath = GetFilePath(userId);
            if (!File.Exists(filePath))
            {
                return new List<Skill>();
            }
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Skill>>(json) ?? new List<Skill>();
        }

        public void SaveAll(int userId, List<Skill> skills)
        {
            var filePath = GetFilePath(userId);
            var json = JsonSerializer.Serialize(skills, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public Skill? GetById(int userId, int id)
        {
            var skills = GetAll(userId);
            return skills.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Додає нову навичку.
        /// </summary>
        public void Add(int userId, Skill skill, IFormFile? file)
        {
                var skills = GetAll(userId);

            // Призначаємо новий ID
            skill.Id = (skills.Any() ? skills.Max(s => s.Id) : 0) + 1;

            // Обробка логотипу
            if (file != null && file.Length > 0)
            {
                skill.LogoPath = SaveImage(file, userId, skill.Id);
            }

            skills.Add(skill);
            SaveAll(userId, skills);
        }

        /// <summary>
        /// Оновлює навичку.
        /// </summary>
        public void Update(int userId, Skill skill, IFormFile? file)
        {
            var skills = GetAll(userId);
            var existing = skills.FirstOrDefault(s => s.Id == skill.Id);
            if (existing != null)
            {
                existing.Title = skill.Title;
                existing.Color = skill.Color;

                // Якщо додається новий файл, замінюємо логотип
                if (file != null && file.Length > 0)
                {
                    DeleteImage(existing.LogoPath); // Видаляємо старий файл
                    existing.LogoPath = SaveImage(file, userId, skill.Id);
                }

                SaveAll(userId, skills);
            }
        }

        /// <summary>
        /// Видаляє навичку.
        /// </summary>
        public void Delete(int userId, int id)
        {
            var skills = GetAll(userId);
            var toRemove = skills.FirstOrDefault(s => s.Id == id);
            if (toRemove != null)
            {
                DeleteImage(toRemove.LogoPath);
                skills.Remove(toRemove);
                SaveAll(userId, skills);
            }
        }

        /// <summary>
        /// Зберігає зображення у wwwroot/images/skills.
        /// </summary>
        private string SaveImage(IFormFile file, int userId, int skillId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "skills");
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
        /// <summary>
        /// Видаляє зображення.
        /// </summary>
        private void DeleteImage(string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}
