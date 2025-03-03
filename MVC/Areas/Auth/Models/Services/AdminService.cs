using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Auth.Models.Forms;
using MVC.Models;
using MVC.Models.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Areas.Auth.Models.Services
{
    public class AdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly SiteContext _siteContext;
        private readonly ProfileService _profileService;

        public AdminService(UserManager<User> userManager, SiteContext siteContext, ProfileService profileService)
        {
            _userManager = userManager;
            _siteContext = siteContext;
            _profileService = profileService;
        }

        public async Task<(List<User> users, Dictionary<int, string> userRoles)> GetUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<int, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            }

            return (users, userRoles);
        }

        public async Task<EditUserForm> GetEditUserFormAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            var roles = await GetAvailableRolesAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            return new EditUserForm
            {
                Email = user.Email,
                FullName = user.FullName,
                ImagePath = user.ImagePath,
                Role = userRoles.FirstOrDefault() ?? "User",
                AvailableRoles = roles
            };
        }

        public async Task<List<string>> GetAvailableRolesAsync()
        {
            return await _siteContext.Roles.Select(x => x.Name).ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(EditUserForm editUserForm)
        {
            var user = await _userManager.FindByEmailAsync(editUserForm.Email);
            if (user == null)
                return false;

            user.FullName = editUserForm.FullName;

            if (editUserForm.Image != null && editUserForm.Image.Length > 0)
            {
                string newImagePath = await _profileService.SaveImage(editUserForm.Image, user.Id);
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    _profileService.DeleteImage(user.ImagePath);
                }
                user.ImagePath = newImagePath;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(editUserForm.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, editUserForm.Role);
            }

            return true;
        }
    }
}