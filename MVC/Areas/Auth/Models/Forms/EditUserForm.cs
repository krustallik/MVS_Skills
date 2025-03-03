using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Auth.Models.Forms
{
    public class EditUserForm
    {
        [Required]
        public string Email { get; set; }

        public string? FullName { get; set; }

        public string? ImagePath { get; set; } // Шлях до зображення

        public IFormFile? Image { get; set; } // Нове зображення для завантаження

        [Required]
        public string Role { get; set; }

        public List<string> AvailableRoles { get; set; } = new();
    }
}
