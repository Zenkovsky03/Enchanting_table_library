using Biblioteka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BookFilesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BookFilesController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(int bookId, string description, IFormFile file)
    {
        description = (description ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            TempData["BookFilesMessage"] = "Podaj opis pliku.";
            return RedirectToAction("Edit", "Books", new { area = "Admin", id = bookId });
        }

        if (file == null || file.Length == 0)
        {
            TempData["BookFilesMessage"] = "Wybierz plik.";
            return RedirectToAction("Edit", "Books", new { area = "Admin", id = bookId });
        }

        var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
        if (!bookExists)
        {
            return NotFound();
        }

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads", "books", bookId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var safeExt = Path.GetExtension(file.FileName);
        var storedName = $"{Guid.NewGuid():N}{safeExt}";
        var fullPath = Path.Combine(uploadsRoot, storedName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/books/{bookId}/{storedName}";

        _context.BookFiles.Add(new BookFile
        {
            BookId = bookId,
            Description = description.Length > 200 ? description.Substring(0, 200) : description,
            FilePath = relativePath,
            UploadedAt = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();

        TempData["BookFilesMessage"] = "Dodano plik.";
        return RedirectToAction("Edit", "Books", new { area = "Admin", id = bookId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.BookFiles.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var bookId = entity.BookId;

        if (!string.IsNullOrWhiteSpace(entity.FilePath) && entity.FilePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            var trimmed = entity.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physical = Path.Combine(_env.WebRootPath ?? string.Empty, trimmed);
            if (System.IO.File.Exists(physical))
            {
                System.IO.File.Delete(physical);
            }
        }

        _context.BookFiles.Remove(entity);
        await _context.SaveChangesAsync();

        TempData["BookFilesMessage"] = "Usunięto plik.";
        return RedirectToAction("Edit", "Books", new { area = "Admin", id = bookId });
    }
}
