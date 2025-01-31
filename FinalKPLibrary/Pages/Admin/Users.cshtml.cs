using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        .ToListAsync();
    }

    public async Task<IActionResult> OnPostAddUserAsync(string username, string password)
    {
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