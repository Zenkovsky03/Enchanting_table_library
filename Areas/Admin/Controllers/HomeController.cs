using Biblioteka.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;

        var model = new AdminDashboardViewModel
        {
            TotalBooks = await _context.Books.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
            TotalLoans = await _context.Loans.CountAsync(),
            ActiveLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Borrowed),
            OverdueLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Borrowed && l.DueDate.HasValue && l.DueDate.Value < now),
            PendingApprovals = await _context.Users.CountAsync(u => !u.IsApproved && u.EmailConfirmed),
            RecentLoans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Take(5)
                .ToListAsync(),
            RecentBooks = await _context.Books
                .OrderByDescending(b => b.AddedDate)
                .Take(5)
                .ToListAsync()
        };

        return View(model);
    }
}

public class AdminDashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalUsers { get; set; }
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public int PendingApprovals { get; set; }
    public List<Loan> RecentLoans { get; set; } = new();
    public List<Book> RecentBooks { get; set; } = new();
}
