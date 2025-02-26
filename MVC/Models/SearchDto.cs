namespace MVC.Models
{
    public class SearchDto
    {
        public string? SearchText { get; set; }
        public List<string>? SelectedProfessions { get; set; }
        public List<int>? SelectedSkills { get; set; }
    }
}
