using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FinalKPLibrary.Pages.Admin;



public class UserDetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public UserDetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Models.User user { get; set; }
    public List<VisibilityArea> AvailableVisibilityAreas { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        user = await _context.Users
            .Include(u => u.UserVisibilityAreas)
            .ThenInclude(uva => uva.VisibilityArea)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (User == null)
        {
            return NotFound();
        }

        AvailableVisibilityAreas = await _context.VisibilityAreas
            .Where(va => !user.UserVisibilityAreas.Select(uva => uva.VisibilityAreaId).Contains(va.Id))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddVisibilityAreaAsync(int userId, int areaId)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var existingLink = await _context.UserVisibilityAreas
            .FirstOrDefaultAsync(uva => uva.UserId == userId && uva.VisibilityAreaId == areaId);

        if (existingLink == null)
        {
            var userVisibilityArea = new UserVisibilityArea
            {
                UserId = userId,
                VisibilityAreaId = areaId
            };

            _context.UserVisibilityAreas.Add(userVisibilityArea);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = userId });
    }

    public async Task<IActionResult> OnPostRemoveVisibilityAreaAsync(int userId, int areaId)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var userVisibilityArea = await _context.UserVisibilityAreas
            .FirstOrDefaultAsync(uva => uva.UserId == userId && uva.VisibilityAreaId == areaId);

        if (userVisibilityArea != null)
        {
            _context.UserVisibilityAreas.Remove(userVisibilityArea);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = userId });
    }
}