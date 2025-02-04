using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FinalKPLibrary.Pages.Admin;

public class VisibilityAreasModel : PageModel
{
    private readonly AppDbContext _context;

    public VisibilityAreasModel(AppDbContext context)
    {
        _context = context;
    }

    public List<VisibilityArea> VisibilityAreas { get; set; }

    public async Task OnGetAsync()
    {
        if (!User.IsInRole("admin"))
        {
            RedirectToPage("/Account/AccessDenied");
        }

        VisibilityAreas = await _context.VisibilityAreas.ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync(string name)
    {
        VisibilityAreas = await _context.VisibilityAreas.ToListAsync();

        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        if (VisibilityAreas == null)
        {
            VisibilityAreas = new List<VisibilityArea>();
        }

        if (VisibilityAreas.Any(v => v.Name == name) )
        {
            ModelState.AddModelError(string.Empty, "An area with that name already exists.");
            return Page();
        }

        var area = new VisibilityArea
        {
            Name = name
        };

        _context.VisibilityAreas.Add(area);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }


        var area = await _context.VisibilityAreas.FindAsync(id);
        if (area != null)
        {
            _context.VisibilityAreas.Remove(area);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}