using Biblioteka.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biblioteka.Services;

public sealed class LoanQueueService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<LoanQueueService> _logger;

    public LoanQueueService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<LoanQueueService> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<int> PromoteWaitingLoansForBookAsync(int bookId)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        var promoted = 0;

        while (true)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                break;
            }

            if (book.StockCount <= 0)
            {
                break;
            }

            var next = await _context.Loans
                .Where(l => l.BookId == bookId && l.Status == LoanStatus.Waiting)
                .OrderBy(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            if (next == null)
            {
                break;
            }

            var user = await _userManager.FindByIdAsync(next.UserId);
            if (user == null || !user.IsActive)
            {
                next.Status = LoanStatus.Cancelled;
                await _context.SaveChangesAsync();
                continue;
            }

            var isReader = await _userManager.IsInRoleAsync(user, "Reader");
            if (isReader && !user.IsApproved)
            {
                next.Status = LoanStatus.Cancelled;
                await _context.SaveChangesAsync();
                continue;
            }

            // Reserve a copy for the next person in queue
            book.StockCount -= 1;
            next.Status = LoanStatus.OnHold;
            next.DueDate ??= DateTime.UtcNow.AddDays(14);
            next.AvailabilityEmailSentAt ??= DateTime.UtcNow;

            await _context.SaveChangesAsync();
            promoted++;

            try
            {
                if (!string.IsNullOrWhiteSpace(user.Email) && user.EmailConfirmed)
                {
                    var subject = "Książka dostępna do odbioru";
                    var due = next.DueDate.HasValue ? next.DueDate.Value.ToString("yyyy-MM-dd") : "(brak)";
                    var body = $"<p>Twoja książka <strong>{System.Net.WebUtility.HtmlEncode(book.Title)}</strong> jest dostępna do odbioru.</p>" +
                               $"<p>Termin odbioru: <strong>{due}</strong>.</p>" +
                               $"<p>Możesz sprawdzić swoje wypożyczenia w zakładce \"Moje wypożyczenia\".</p>";

                    await _emailSender.SendEmailAsync(user.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send availability email for LoanId={LoanId}", next.Id);
            }
        }

        await tx.CommitAsync();
        return promoted;
    }
}
