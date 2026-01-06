using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Data;
namespace Biblioteka.Controllers
{
    [Authorize]
    public class SavedSearchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavedSearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var searches = await _context.SavedSearches
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(searches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string name, string queryData)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            name = (name ?? string.Empty).Trim();
            queryData = (queryData ?? string.Empty).Trim().TrimStart('?');

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["SavedSearchMessage"] = "Podaj nazwę wyszukiwania.";
                return RedirectToAction("Advanced", "Search", new { });
            }

            if (string.IsNullOrWhiteSpace(queryData))
            {
                TempData["SavedSearchMessage"] = "Brak parametrów wyszukiwania do zapisania.";
                return RedirectToAction("Advanced", "Search", new { });
            }

            var entity = new SavedSearch
            {
                UserId = userId,
                Name = name.Length > 100 ? name.Substring(0, 100) : name,
                QueryData = queryData,
                CreatedAt = DateTime.UtcNow,
            };

            _context.SavedSearches.Add(entity);
            await _context.SaveChangesAsync();

            TempData["SavedSearchMessage"] = "Zapisano wyszukiwanie.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Run(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var saved = await _context.SavedSearches
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (saved == null)
            {
                return NotFound();
            }

            var qs = (saved.QueryData ?? string.Empty).Trim().TrimStart('?');
            return Redirect($"/Search/Advanced?{qs}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var saved = await _context.SavedSearches
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (saved == null)
            {
                return NotFound();
            }

            _context.SavedSearches.Remove(saved);
            await _context.SaveChangesAsync();

            TempData["SavedSearchMessage"] = "Usunięto zapisane wyszukiwanie.";
            return RedirectToAction(nameof(Index));
        }
    }
}
