using Biblioteka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Books.Include(b => b.Category);
        return View(await applicationDbContext.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
            .Include(b => b.Files)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View("~/Views/Books/Details.cshtml", book);
    }

    public IActionResult Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
        return View("~/Views/Books/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,Description,TableOfContentsExcerpt,AddedDate,IsNew,StockCount,CategoryId")] Book book, int[] selectedTags)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();

            if (selectedTags != null && selectedTags.Length > 0)
            {
                foreach (var tagId in selectedTags)
                {
                    _context.BookTags.Add(new BookTag { BookId = book.Id, TagId = tagId });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTags);
        return View("~/Views/Books/Create.cshtml", book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Files)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
        var selectedTagIds = _context.BookTags.Where(bt => bt.BookId == book.Id).Select(bt => bt.TagId).ToList();
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTagIds);

        return View("~/Views/Books/Edit.cshtml", book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,Description,TableOfContentsExcerpt,AddedDate,IsNew,StockCount,CategoryId")] Book book, int[] selectedTags)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();

                var existing = _context.BookTags.Where(bt => bt.BookId == book.Id);
                _context.BookTags.RemoveRange(existing);
                if (selectedTags != null && selectedTags.Length > 0)
                {
                    foreach (var tagId in selectedTags)
                    {
                        _context.BookTags.Add(new BookTag { BookId = book.Id, TagId = tagId });
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == book.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTags);
        return View("~/Views/Books/Edit.cshtml", book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View("~/Views/Books/Delete.cshtml", book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
