using Microsoft.AspNetCore.Http;
using MVC.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MVC.Areas.Auth.Models.Services
{
    public class ProfileService
    {
        private readonly SiteContext _context;

        public ProfileService(SiteContext context)
        {
            _context = context;
        }

        public async Task UpdateProfileAsync(User user, IFormFile? imageFile, string? fullName, string? phone)
        {
            user.FullName = fullName;
            user.PhoneNumber = phone;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Якщо є попереднє зображення, видаляємо його
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    DeleteImage(user.ImagePath);
                }
                string filePath = await SaveImage(imageFile, user.Id);
                user.ImagePath = filePath;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<string> SaveImage(IFormFile file, int userId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
            {
                throw new InvalidOperationException("Файл повинен бути зображенням (.jpg, .jpeg, .png, .gif)");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"user_{userId}_{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/avatars/{uniqueFileName}";
        }


        public void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}
