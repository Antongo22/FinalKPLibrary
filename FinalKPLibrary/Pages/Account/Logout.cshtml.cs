using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FinalKPLibrary.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Выход из системы
            await _signInManager.SignOutAsync();

            // Удаление кук аутентификации
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Перенаправление на главную страницу
            return RedirectToPage("/Index");
        }
    }
}
