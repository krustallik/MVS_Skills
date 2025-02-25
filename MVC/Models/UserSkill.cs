

using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
    public class UserSkill
    {
        public int UserInfoId { get; set; }
        public int SkillId { get; set; }

        public virtual UserInfo UserInfo { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}