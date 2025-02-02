using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalKPLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DocumentsModel : PageModel
{
    private readonly AppDbContext _context;

    public DocumentsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Doc> Documents { get; set; } = new List<Doc>();
    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var query = _context.Set<Doc>().Include(d => d.DocVisibilityAreas).ThenInclude(dva => dva.VisibilityArea).AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            query = query.Where(d =>
                d.Name.Contains(SearchQuery) ||
                d.Description.Contains(SearchQuery) ||
                d.Topic.Contains(SearchQuery) ||
                d.UploadDate.ToString().Contains(SearchQuery) ||
                d.DocVisibilityAreas.Any(dva => dva.VisibilityArea.Name.Contains(SearchQuery))
            );
        }

        Documents = await query.ToListAsync();
        return Page();
    }
}
