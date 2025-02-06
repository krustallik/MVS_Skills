namespace MVC.Models;

public class Skill
{
    public Skill(string Title, int Level) {
        this.Title = Title;
        this.Level = Level;
    }
    public string Title { get; set; }
    public int Level { get; set; }
}
