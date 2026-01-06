using System.Diagnostics;
using Biblioteka.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _context.News
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishDate)
                .Take(5)
                .ToListAsync();

            var newBooks = await _context.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.IsNew)
                .ThenByDescending(b => b.AddedDate)
                .Take(8)
                .ToListAsync();

            return View(new HomeIndexViewModel
            {
                News = news,
                NewBooks = newBooks,
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
