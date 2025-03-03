using Microsoft.AspNetCore.Http;

namespace MVC.Areas.Auth.Models.Forms
{
    public class ProfileForm
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        // Поле для завантаження аватарки
        public IFormFile? Image { get; set; }
        // Поточний шлях до зображення (для відображення)
        public string? ImagePath { get; set; }
    }
}
