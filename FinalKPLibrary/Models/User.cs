using FinalKPLibrary.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<int>
{
    public string Type { get; set; } // "user" или "admin"
    public ICollection<UserVisibilityArea> UserVisibilityAreas { get; set; } = new List<UserVisibilityArea>();
}