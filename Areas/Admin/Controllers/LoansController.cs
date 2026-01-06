using Biblioteka.Areas.Admin.Models;
using Biblioteka.Data;
using Biblioteka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Employee")]
public class LoansController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly LoanQueueService _loanQueue;

    public LoansController(ApplicationDbContext context, LoanQueueService loanQueue)
    {
        _context = context;
        _loanQueue = loanQueue;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, int? status, bool overdueOnly = false)
    {
        var query = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(l =>
                l.Book != null && (
                    l.Book.Title.Contains(term)
                    || l.Book.Author.Contains(term)
                    || l.Book.Isbn.Contains(term)
                )
                || (l.User != null && (
                    l.User.Email!.Contains(term)
                    || l.User.UserName!.Contains(term)
                )));
        }

        if (status.HasValue)
        {
            query = query.Where(l => (int)l.Status == status.Value);
        }

        if (overdueOnly)
        {
            var now = DateTime.UtcNow;
            query = query.Where(l => l.Status == LoanStatus.Borrowed && l.DueDate.HasValue && l.DueDate.Value < now);
        }

        var model = new AdminLoansIndexViewModel
        {
            Q = q,
            Status = status,
            OverdueOnly = overdueOnly,
            Loans = await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(),
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, LoanStatus status)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        if (loan.Status == LoanStatus.Returned && status != LoanStatus.Returned)
        {
            TempData["AdminLoansMessage"] = "Nie można zmienić statusu po zwrocie.";
            return RedirectToAction(nameof(Index));
        }

        var previousStatus = loan.Status;
        loan.Status = status;

        var now = DateTime.UtcNow;

        if (status == LoanStatus.Borrowed && loan.BorrowedAt == null)
        {
            loan.BorrowedAt = now;
            loan.DueDate ??= now.AddDays(14);
        }

        if (status == LoanStatus.Returned)
        {
            loan.ReturnedAt ??= now;

            if (previousStatus != LoanStatus.Returned)
            {
                if (loan.Book != null)
                {
                    loan.Book.StockCount += 1;
                }
            }
        }

        if (status == LoanStatus.Cancelled && previousStatus != LoanStatus.Cancelled)
        {
            // If a reserved/borrowed copy is being freed, return it to stock.
            if (loan.Book != null)
            {
                if (previousStatus == LoanStatus.OnHold)
                {
                    loan.Book.StockCount += 1;
                }
                else if (previousStatus == LoanStatus.Borrowed)
                {
                    loan.Book.StockCount += 1;
                    loan.ReturnedAt ??= now;
                }
            }
        }

        await _context.SaveChangesAsync();

        // Auto-promote the first waiting reader after any event freeing a copy.
        if (loan.BookId != 0 && (status == LoanStatus.Returned || status == LoanStatus.Cancelled))
        {
            await _loanQueue.PromoteWaitingLoansForBookAsync(loan.BookId);
        }

        TempData["AdminLoansMessage"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Index));
    }
}
