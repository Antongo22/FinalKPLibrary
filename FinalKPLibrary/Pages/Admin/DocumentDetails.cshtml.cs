using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalKPLibrary.Pages.Admin;


public class DocumentDetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DocumentDetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Doc Doc { get; set; }
    public List<VisibilityArea> AvailableVisibilityAreas { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // Загружаем документ с его зонами видимости
        Doc = await _context.Docs
            .Include(d => d.DocVisibilityAreas)
            .ThenInclude(dva => dva.VisibilityArea)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (Doc == null)
        {
            return NotFound();
        }

        // Загружаем все доступные зоны видимости
        AvailableVisibilityAreas = await _context.VisibilityAreas
            .Where(va => !Doc.DocVisibilityAreas.Select(dva => dva.VisibilityAreaId).Contains(va.Id))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddVisibilityAreaAsync(int docId, int areaId)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // Проверяем, что зона видимости ещё не присвоена документу
        var existingLink = await _context.DocVisibilityAreas
            .FirstOrDefaultAsync(dva => dva.DocId == docId && dva.VisibilityAreaId == areaId);

        if (existingLink == null)
        {
            // Создаем связь между документом и зоной видимости
            var docVisibilityArea = new DocVisibilityArea
            {
                DocId = docId,
                VisibilityAreaId = areaId
            };

            _context.DocVisibilityAreas.Add(docVisibilityArea);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = docId });
    }

    public async Task<IActionResult> OnPostRemoveVisibilityAreaAsync(int docId, int areaId)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // Находим связь между документом и зоной видимости
        var docVisibilityArea = await _context.DocVisibilityAreas
            .FirstOrDefaultAsync(dva => dva.DocId == docId && dva.VisibilityAreaId == areaId);

        if (docVisibilityArea != null)
        {
            // Удаляем связь
            _context.DocVisibilityAreas.Remove(docVisibilityArea);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = docId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("admin"))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        // Находим документ
        var doc = await _context.Docs.FindAsync(id);
        if (doc != null)
        {
            // Удаляем документ
            _context.Docs.Remove(doc);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Admin/Documents");
    }
}