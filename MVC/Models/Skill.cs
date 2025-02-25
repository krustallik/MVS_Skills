using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов’язковою")]
        [StringLength(100, ErrorMessage = "Максимальна довжина назви – 100 символів")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Колір є обов’язковим")]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Невірний формат кольору (наприклад, #FFFFFF)")]
        public string Color { get; set; } = "#FFFFFF";

        // Поле Level видаляється з цієї моделі!

        public string? LogoPath { get; set; }

        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }

}
