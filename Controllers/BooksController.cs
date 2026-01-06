using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Data;
using Microsoft.AspNetCore.Authorization;
using Biblioteka.Infrastructure;
using Biblioteka.Models;
using Microsoft.AspNetCore.Identity;
using Biblioteka.Services;
namespace Biblioteka.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LoanQueueService _loanQueue;

        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, LoanQueueService loanQueue)
        {
            _context = context;
            _userManager = userManager;
            _loanQueue = loanQueue;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? q = null, int? categoryId = null, int? tagId = null)
        {
            var booksQuery = _context.Books
                .Include(b => b.Category)
                .Include(b => b.BookTags)
                    .ThenInclude(bt => bt.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                booksQuery = booksQuery.Where(b =>
                    (b.Title != null && b.Title.Contains(term)) ||
                    (b.Author != null && b.Author.Contains(term)) ||
                    (b.Isbn != null && b.Isbn.Contains(term)));
            }

            if (categoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);
            }

            if (tagId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.BookTags.Any(bt => bt.TagId == tagId.Value));
            }

            var model = new BooksBrowseViewModel
            {
                Q = q,
                CategoryId = categoryId,
                TagId = tagId,
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                Tags = await _context.Tags.OrderBy(t => t.Name).ToListAsync(),
                Books = await booksQuery.OrderBy(b => b.Title).ToListAsync(),
            };

            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            model.CartCount = cart.Sum(x => x.Quantity);

            return View(model);
        }

        // GET: Books/Details/5
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

            return View(book);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinQueue(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (!user.IsActive)
            {
                return Forbid();
            }

            var isReader = await _userManager.IsInRoleAsync(user, "Reader");
            if (isReader && !user.IsApproved)
            {
                return Forbid();
            }

            var alreadyHas = await _context.Loans.AnyAsync(l =>
                l.BookId == id
                && l.UserId == user.Id
                && (l.Status == LoanStatus.Waiting || l.Status == LoanStatus.OnHold || l.Status == LoanStatus.Borrowed));

            if (alreadyHas)
            {
                TempData["BooksMessage"] = "Masz już tę książkę w kolejce lub wypożyczoną.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.Loans.Add(new Loan
            {
                BookId = id,
                UserId = user.Id,
                Status = LoanStatus.Waiting,
                CreatedAt = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();

            // If a copy is available right now, promote immediately.
            await _loanQueue.PromoteWaitingLoansForBookAsync(id);

            TempData["BooksMessage"] = "Dodano do kolejki.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);

            var selectedTagIds = _context.BookTags.Where(bt => bt.BookId == book.Id).Select(bt => bt.TagId).ToList();
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTagIds);

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

                    // update tags: remove existing and add selected
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
                    if (!BookExists(book.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTags);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
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

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
