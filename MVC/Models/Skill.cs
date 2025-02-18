using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва є обов’язковою")]
        [StringLength(100, ErrorMessage = "Максимальна довжина назви – 100 символів")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Колір є обов’язковим")]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Невірний формат кольору (очікується, наприклад, #FFFFFF)")]
        public string Color { get; set; } = "#FFFFFF";

        public string? LogoPath { get; set; }
    }
}
