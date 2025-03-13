namespace MVC.Models;

public class UserRating
{
    // Ідентифікатор профілю, який оцінюється
    public int UserInfoId { get; set; }
    public virtual UserInfo UserInfo { get; set; } = null!;

    // Ідентифікатор користувача, який голосує
    public int RaterId { get; set; }
    public virtual User Rater { get; set; } = null!;

    // Значення оцінки, наприклад від 1 до 5
    public int Rating { get; set; }
}
