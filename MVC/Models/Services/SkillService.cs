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

        public void Add(int userId, Skill skill)
        {
            var skills = GetAll(userId);
            // Призначаємо новий Id (послідовне зростання)
            skill.Id = (skills.Any() ? skills.Max(s => s.Id) : 0) + 1;
            skills.Add(skill);
            SaveAll(userId, skills);
        }

        public void Update(int userId, Skill skill)
        {
            var skills = GetAll(userId);
            var existing = skills.FirstOrDefault(s => s.Id == skill.Id);
            if (existing != null)
            {
                existing.Title = skill.Title;
                existing.Color = skill.Color;
                SaveAll(userId, skills);
            }
        }

        public void Delete(int userId, int id)
        {
            var skills = GetAll(userId);
            var toRemove = skills.FirstOrDefault(s => s.Id == id);
            if (toRemove != null)
            {
                skills.Remove(toRemove);
                SaveAll(userId, skills);
            }
        }
    }
}
