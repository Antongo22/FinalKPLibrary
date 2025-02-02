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

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortOrder { get; set; } = "asc";
    public string NextSortOrder => SortOrder == "asc" ? "desc" : "asc";

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

        if (!string.IsNullOrWhiteSpace(SortBy))
        {
            query = SortOrder switch
            {
                "asc" => SortBy switch
                {
                    "Name" => query.OrderBy(d => d.Name),
                    "Description" => query.OrderBy(d => d.Description),
                    "Topic" => query.OrderBy(d => d.Topic),
                    "UploadDate" => query.OrderBy(d => d.UploadDate),
                    _ => query
                },
                "desc" => SortBy switch
                {
                    "Name" => query.OrderByDescending(d => d.Name),
                    "Description" => query.OrderByDescending(d => d.Description),
                    "Topic" => query.OrderByDescending(d => d.Topic),
                    "UploadDate" => query.OrderByDescending(d => d.UploadDate),
                    _ => query
                },
                _ => query
            };
        }

        Documents = await query.ToListAsync();
        return Page();
    }
}
