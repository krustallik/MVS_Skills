using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
    public class SiteContext : DbContext
    {
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }

        public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Встановлення складеного ключа для UserSkill
            modelBuilder.Entity<UserSkill>()
                .HasKey(us => new { us.UserInfoId, us.SkillId });

            // Визначення зв’язків M:N
            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.UserInfo)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserInfoId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);
        }
    }
}
