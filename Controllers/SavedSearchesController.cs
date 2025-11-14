using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers
{
    public class SavedSearchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavedSearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SavedSearches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SavedSearches.Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SavedSearches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedSearch = await _context.SavedSearches
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedSearch == null)
            {
                return NotFound();
            }

            return View(savedSearch);
        }

        // GET: SavedSearches/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: SavedSearches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,QueryData,CreatedAt")] SavedSearch savedSearch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(savedSearch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", savedSearch.UserId);
            return View(savedSearch);
        }

        // GET: SavedSearches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedSearch = await _context.SavedSearches.FindAsync(id);
            if (savedSearch == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", savedSearch.UserId);
            return View(savedSearch);
        }

        // POST: SavedSearches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,QueryData,CreatedAt")] SavedSearch savedSearch)
        {
            if (id != savedSearch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedSearch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedSearchExists(savedSearch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", savedSearch.UserId);
            return View(savedSearch);
        }

        // GET: SavedSearches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedSearch = await _context.SavedSearches
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedSearch == null)
            {
                return NotFound();
            }

            return View(savedSearch);
        }

        // POST: SavedSearches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var savedSearch = await _context.SavedSearches.FindAsync(id);
            if (savedSearch != null)
            {
                _context.SavedSearches.Remove(savedSearch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedSearchExists(int id)
        {
            return _context.SavedSearches.Any(e => e.Id == id);
        }
    }
}
