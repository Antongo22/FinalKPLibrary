using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace FinalKPLibrary.Pages.Admin;

public class UsersModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public UsersModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public List<User> Users { get; set; }

    public async Task OnGetAsync()
    {
        if (!User.IsInRole("admin"))
        {
            RedirectToPage("/Account/AccessDenied");
        }
        
        Users = await _userManager.Users
        .Where(u => u.Type == "user")
        .Include(d => d.UserVisibilityAreas) 
        .ThenInclude(uva => uva.VisibilityArea)
        .ToListAsync();
    }

    public async Task<IActionResult> OnPostAddUserAsync(string username, string password)
    {
        if (!User.IsInRole("admin"))
        {
            RedirectToPage("/Account/AccessDenied");
        }

        Users = await _userManager.Users
        .Where(u => u.Type == "user")
        .Include(d => d.UserVisibilityAreas)
        .ThenInclude(uva => uva.VisibilityArea)
        .ToListAsync();

        if (Users == null)
        {
            Users = new List<User>();
        }

        if (Users.Any(u => u.UserName == username) || username.ToString().Contains("admin"))
        {
            ModelState.AddModelError(string.Empty, "ѕользователь с таким именем уже существует.");
            return Page(); 
        }


        var user = new User { UserName = username, Type = "user" };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "user");
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToPage();
    }
}