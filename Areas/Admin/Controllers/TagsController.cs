using Biblioteka.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TagsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TagsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View("~/Views/Tags/Index.cshtml", await _context.Tags.OrderBy(t => t.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
        if (tag == null) return NotFound();

        return View("~/Views/Tags/Details.cshtml", tag);
    }

    public IActionResult Create()
    {
        return View("~/Views/Tags/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] Tag tag)
    {
        if (ModelState.IsValid)
        {
            _context.Add(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Tags/Create.cshtml", tag);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        return View("~/Views/Tags/Edit.cshtml", tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Tag tag)
    {
        if (id != tag.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(tag);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tags.Any(e => e.Id == tag.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Tags/Edit.cshtml", tag);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
        if (tag == null) return NotFound();

        return View("~/Views/Tags/Delete.cshtml", tag);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
