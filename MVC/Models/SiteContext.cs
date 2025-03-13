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
        public DbSet<UserRating> UserRatings { get; set; }


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

            // Налаштування composite key для UserRating
            modelBuilder.Entity<UserRating>()
                .HasKey(ur => new { ur.UserInfoId, ur.RaterId });

            // Налаштування зв’язку UserRating - UserInfo
            modelBuilder.Entity<UserRating>()
                .HasOne(ur => ur.UserInfo)
                .WithMany(ui => ui.UserRatings)
                .HasForeignKey(ur => ur.UserInfoId);

            // Налаштування зв’язку UserRating - User (голосуючого)
            modelBuilder.Entity<UserRating>()
                .HasOne(ur => ur.Rater)
                .WithMany() // або, якщо потрібна колекція голосів у User, додайте відповідну властивість
                .HasForeignKey(ur => ur.RaterId);
        }

    }
}
