namespace MVC.Models
{
    public sealed class SearchProfileViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Profession { get; init; }
        public int ExperienceYears { get; init; }
        public List<string> Skills { get; init; } = [];
        public int SkillCount { get; init; }
        public double? AverageRating { get; init; }
        public int RatingCount { get; init; }
    }
}