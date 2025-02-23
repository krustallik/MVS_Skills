using MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models.Services
{
    public class UserInfoService
    {
        private readonly SiteContext _context;
        public UserInfoService(SiteContext context)
        {
            _context = context;
        }

        public async Task<List<UserInfo>> GetAllAsync()
        {
            return await _context.UserInfos.Include(u => u.Skills).ToListAsync();
        }

        public async Task<UserInfo?> FindByIdAsync(int id)
        {
            return await _context.UserInfos.Include(u => u.Skills)
                                           .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(UserInfo user)
        {
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var user = await FindByIdAsync(id);
            if (user != null)
            {
                _context.UserInfos.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Додано метод SaveChangesAsync для збереження змін
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
