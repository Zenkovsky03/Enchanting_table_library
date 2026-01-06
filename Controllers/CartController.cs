using Biblioteka.Data;
using Biblioteka.Infrastructure;
using Biblioteka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        var bookIds = cart.Select(c => c.BookId).Distinct().ToList();

        var books = await _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id);

        var model = new CartIndexViewModel
        {
            Lines = cart
                .Where(c => books.ContainsKey(c.BookId))
                .Select(c =>
                {
                    var book = books[c.BookId];
                    return new CartLineViewModel
                    {
                        BookId = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        StockCount = book.StockCount,
                        Quantity = c.Quantity
                    };
                })
                .OrderBy(l => l.Title)
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        if (quantity < 1) quantity = 1;

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        if (book.StockCount <= 0)
        {
            ModelState.AddModelError(string.Empty, $"Książka '{book.Title}' jest aktualnie niedostępna.");
            return await Index();
        }

        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.BookId == id);
        if (existing == null)
        {
            cart.Add(new CartItem { BookId = id, Quantity = Math.Min(quantity, book.StockCount) });
        }
        else
        {
            var newQty = existing.Quantity + quantity;
            existing.Quantity = Math.Min(newQty, book.StockCount);
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.BookId == id);
        if (existing != null)
        {
            cart.Remove(existing);
            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decrement(int id)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.BookId == id);
        if (existing != null)
        {
            existing.Quantity -= 1;
            if (existing.Quantity <= 0)
            {
                cart.Remove(existing);
            }
            SaveCart(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        SaveCart(new List<CartItem>());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (cart.Count == 0)
        {
            return RedirectToAction(nameof(Index));
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

        var bookIds = cart.Select(c => c.BookId).Distinct().ToList();
        var books = await _context.Books.Where(b => bookIds.Contains(b.Id)).ToDictionaryAsync(b => b.Id);

        foreach (var item in cart)
        {
            if (!books.TryGetValue(item.BookId, out var book))
            {
                ModelState.AddModelError(string.Empty, "Jedna z książek w koszyku nie istnieje.");
                return await Index();
            }

            if (book.StockCount < item.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"Brak wystarczającej liczby egzemplarzy: {book.Title} (dostępne: {book.StockCount}).");
                return await Index();
            }
        }

        using var tx = await _context.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;
        var created = 0;

        foreach (var item in cart)
        {
            var book = books[item.BookId];
            book.StockCount -= item.Quantity;

            for (var i = 0; i < item.Quantity; i++)
            {
                _context.Loans.Add(new Loan
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    Status = LoanStatus.OnHold,
                    CreatedAt = now,
                    DueDate = now.AddDays(14)
                });
                created++;
            }
        }

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        SaveCart(new List<CartItem>());

        return View("CheckoutResult", new CheckoutResultViewModel { CreatedLoans = created });
    }

    private List<CartItem> GetCart()
    {
        return HttpContext.Session.GetJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetJson(CartSessionKey, cart);
    }
}
