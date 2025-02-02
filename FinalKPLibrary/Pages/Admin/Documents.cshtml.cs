using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FinalKPLibrary.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FinalKPLibrary.Pages.Admin
{
    public class DocumentsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DocumentsModel> _logger;

        public DocumentsModel(AppDbContext context, IWebHostEnvironment environment, ILogger<DocumentsModel> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
        }

        public List<Doc> Documents { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        public async Task OnGetAsync()
        {
            if (!User.IsInRole("admin"))
            {
                RedirectToPage("/Account/AccessDenied");
            }

            var query = _context.Docs
                .Include(d => d.DocVisibilityAreas)
                .ThenInclude(dva => dva.VisibilityArea)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(d =>
                    d.Name.Contains(SearchQuery) ||
                    d.Description.Contains(SearchQuery) ||
                    d.Topic.Contains(SearchQuery) ||
                    d.UploadDate.ToString().Contains(SearchQuery) ||
                    d.DocVisibilityAreas.Any(dva => dva.VisibilityArea.Name.Contains(SearchQuery)));
            }

            Documents = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostUploadAsync(string name, string description, string topic, IFormFile file)
        {
            if (!User.IsInRole("admin"))
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            if (_context.Docs.Any(d => d.Name == name))
            {
                ModelState.AddModelError(string.Empty, "ƒокумент с таким именем уже существует.");
                return Page();
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file.");
                return Page();
            }

            var allowedExtensions = new[] { ".docx", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("file", "Only .docx and .pdf files are allowed.");
                return Page();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var doc = new Doc
            {
                Name = name,
                Description = description,
                Topic = topic,
                UploadDate = DateTime.Now,
                FilePath = filePath
            };

            _context.Docs.Add(doc);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!User.IsInRole("admin"))
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            var doc = await _context.Docs.FindAsync(id);
            if (doc != null)
            {
                if (System.IO.File.Exists(doc.FilePath))
                {
                    System.IO.File.Delete(doc.FilePath);
                }

                _context.Docs.Remove(doc);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}