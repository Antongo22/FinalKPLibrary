using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public class DocumentsModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public DocumentsModel(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Doc> Documents { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        var userVisibilityAreas = await _context.UserVisibilityAreas
            .Where(uva => uva.UserId == user.Id)
            .Select(uva => uva.VisibilityAreaId)
            .ToListAsync();

        Documents = await _context.Docs
            .Include(d => d.DocVisibilityAreas)
            .ThenInclude(dva => dva.VisibilityArea)
            .Where(d => d.DocVisibilityAreas.Any(dva => userVisibilityAreas.Contains(dva.VisibilityAreaId)))
            .ToListAsync();
    }
}