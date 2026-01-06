using Biblioteka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NewsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View("~/Views/News/Index.cshtml", await _context.News.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
        if (news == null) return NotFound();
        return View("~/Views/News/Details.cshtml", news);
    }

    public IActionResult Create()
    {
        return View("~/Views/News/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Content,PublishDate,IsPublished")] News news)
    {
        if (ModelState.IsValid)
        {
            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View("~/Views/News/Create.cshtml", news);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var news = await _context.News.FindAsync(id);
        if (news == null) return NotFound();
        return View("~/Views/News/Edit.cshtml", news);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PublishDate,IsPublished")] News news)
    {
        if (id != news.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(news);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.News.Any(e => e.Id == news.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/News/Edit.cshtml", news);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
        if (news == null) return NotFound();
        return View("~/Views/News/Delete.cshtml", news);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            _context.News.Remove(news);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
