using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace MVC.Models
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public string? ImagePath { get; set; }
    }
}
