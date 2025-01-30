using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FinalKPLibrary.Pages.Account;


public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public LoginModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Username, Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }
        }

        return Page();
    }
}