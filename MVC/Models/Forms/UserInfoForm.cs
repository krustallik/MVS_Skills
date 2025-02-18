using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MVC.Models;

namespace MVC.Models.Forms;

public class UserInfoForm
{
    public UserInfoForm() { }

    public UserInfoForm(UserInfo model)
    {
        Name = model.Name;
        Email = model.Email;
        Description = model.Description;
        Birthday = model.Birthday;
        IsActive = model.IsActive;
        ExpirienseYears = model.ExpirienseYears;
        Salary = model.Salary;
        if (model.Profession != null)
        {
            ProfessionId = Professions.FindIndex(x => x == model.Profession);
        }
    }

    public void Update(UserInfo model)
    {
        model.Name = Name;
        model.Email = Email;
        model.Description = Description;
        model.Birthday = Birthday;
        model.IsActive = IsActive;
        model.ExpirienseYears = ExpirienseYears;
        model.Salary = Salary;
        model.Profession = Professions[ProfessionId ?? 0];
    }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name length must be between 2 and 100 characters", MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Description length cannot exceed 500 characters")]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Birthday is required")]
    [CustomValidation(typeof(UserInfoForm), nameof(ValidateBirthday))]
    public DateTime Birthday { get; set; }

    public bool IsActive { get; set; }

    [Range(0, 100, ErrorMessage = "Experience must be between 0 and 100 years")]
    public int ExpirienseYears { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
    public decimal Salary { get; set; }

    [Range(0, 9, ErrorMessage = "Invalid profession selection")]
    public int? ProfessionId { get; set; }

    public List<string> Professions => [
        "Software Engineer",
        "Data Scientist",
        "Mechanical Engineer",
        "Electrical Technician",
        "Civil Engineer",
        "Graphic Designer",
        "Doctor",
        "Teacher",
        "Marketing Specialist",
        "Project Manager"
    ];

    [DisplayName("Avatar")]
    public IFormFile? Image { get; set; }

    [Display(Name = "Фотографії")]
    public List<IFormFile>? Photos { get; set; }

    public static ValidationResult? ValidateBirthday(DateTime birthday, ValidationContext context)
    {
        if (birthday > DateTime.Now)
        {
            return new ValidationResult("Birthday cannot be in the future");
        }
        return ValidationResult.Success;
    }

}
