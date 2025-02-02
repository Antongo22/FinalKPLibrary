using Microsoft.AspNetCore.Mvc;
using FinalKPLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace FinalKPLibrary.Controllers
{
    [Route("files")]
    public class FilesController : Controller
    {
        private readonly AppDbContext _context;
        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFile(int id)
        {
            // Получаем документ из БД по id
            var doc = await _context.Set<Doc>().FindAsync(id);
            if (doc == null)
            {
                return NotFound("Документ не найден.");
            }

            string relativePath = doc.FilePath;
            // Если в пути содержится "wwwroot", то берем все, что после него
            var marker = "wwwroot";
            var index = doc.FilePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                relativePath = doc.FilePath.Substring(index + marker.Length);
            }
            // Убираем начальные разделители
            relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Формируем абсолютный путь к файлу
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден на сервере.");
            }

            // Определяем MIME-тип файла
            string contentType = "application/octet-stream";
            if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/pdf";
            }
            else if (filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }

            return PhysicalFile(filePath, contentType);
        }
    }
}
