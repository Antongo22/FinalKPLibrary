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
            // ����� �� �������
            await _signInManager.SignOutAsync();

            // �������� ��� ��������������
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // ��������������� �� ������� ��������
            return RedirectToPage("/Index");
        }
    }
}
