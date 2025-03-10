using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
    public class SiteContext : IdentityDbContext<User,IdentityRole<int>, int>
    {
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }

        public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування складеного ключа для UserSkill
            modelBuilder.Entity<UserSkill>()
                .HasKey(us => new { us.UserInfoId, us.SkillId });

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.UserInfo)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserInfoId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);

            // Налаштування зв’язку UserInfo - User (власника)
            modelBuilder.Entity<UserInfo>()
                .HasOne(ui => ui.Owner)
                .WithMany() // або, якщо у користувача є колекція записів, наприклад, public ICollection<UserInfo> UserInfos { get; set; }
                .HasForeignKey(ui => ui.OwnerId);
        }

    }
}
