using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
    public class SiteContext : DbContext
    {
        public SiteContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
    }
}
