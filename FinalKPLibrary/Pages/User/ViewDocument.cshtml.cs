using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;

namespace FinalKPLibrary.Pages.User
{
    public class ViewDocumentModel : PageModel
    {
        private readonly AppDbContext _context;

        public ViewDocumentModel(AppDbContext context)
        {
            _context = context;
        }

        public Doc Document { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Document = await _context.Set<Doc>()
                .Include(d => d.DocVisibilityAreas)
                    .ThenInclude(dva => dva.VisibilityArea)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (Document == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
