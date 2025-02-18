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

        // Нова властивість – рівень володіння (наприклад, від 1 до 100)
        [Required(ErrorMessage = "Вкажіть рівень володіння навичкою")]
        [Range(1, 100, ErrorMessage = "Рівень має бути від 1 до 100")]
        public int Level { get; set; }

        public string? LogoPath { get; set; }
    }
}
