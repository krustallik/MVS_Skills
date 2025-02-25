using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models.Services
{
    public class UserInfoService
    {
        private readonly SiteContext _context;
        public UserInfoService(SiteContext context)
        {
            _context = context;
        }

        // Отримати всіх користувачів з їхніми навичками
        public async Task<List<UserInfo>> GetAllAsync()
        {
            return await _context.UserInfos
                                 .Include(u => u.UserSkills)
                                 .ThenInclude(us => us.Skill)
                                 .ToListAsync();
        }

        // Знайти користувача за ID разом з його навичками
        public async Task<UserInfo?> FindByIdAsync(int id)
        {
            return await _context.UserInfos
                                 .Include(u => u.UserSkills)
                                 .ThenInclude(us => us.Skill)
                                 .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Додати нового користувача
        public async Task AddAsync(UserInfo user)
        {
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();
        }

        // Видалити користувача за ID
        public async Task DeleteByIdAsync(int id)
        {

            var user = await FindByIdAsync(id);
            if (user != null)
            {
                _context.UserInfos.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Додати навичку користувачеві
        public async Task AddSkillToUserAsync(int userId, int skillId)
        {
            var user = await _context.UserInfos.FindAsync(userId);
            var skill = await _context.Skills.FindAsync(skillId);

            if (user != null && skill != null)
            {
                var userSkill = new UserSkill
                {
                    UserInfoId = userId,
                    SkillId = skillId
                };

                _context.UserSkills.Add(userSkill);
                await _context.SaveChangesAsync();
            }
        }

        // Видалити навичку у користувача
        public async Task RemoveSkillFromUserAsync(int userId, int skillId)
        {
            var userSkill = await _context.UserSkills
                                          .FirstOrDefaultAsync(us => us.UserInfoId == userId && us.SkillId == skillId);

            if (userSkill != null)
            {
                _context.UserSkills.Remove(userSkill);
                await _context.SaveChangesAsync();
            }
        }

        // Оновити дані користувача
        public async Task UpdateAsync(UserInfo user)
        {
            _context.UserInfos.Update(user);
            await _context.SaveChangesAsync();
        }

        // Зберегти зміни
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
