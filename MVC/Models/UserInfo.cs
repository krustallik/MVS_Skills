namespace MVC.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Description { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsActive { get; set; }
        public int ExpirienseYears { get; set; }
        public decimal Salary { get; set; }
        public string? Profession { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public List<string> PhotoPaths { get; set; } = new List<string>();
        public string? AvatarPhoto { get; set; }
    }
}
