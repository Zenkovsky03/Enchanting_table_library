using Biblioteka.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Biblioteka.Services;

public sealed class LoanNotificationsHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<LoanNotificationsOptions> _options;
    private readonly ILogger<LoanNotificationsHostedService> _logger;

    public LoanNotificationsHostedService(
        IServiceScopeFactory scopeFactory,
        IOptions<LoanNotificationsOptions> options,
        ILogger<LoanNotificationsHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scanIntervalMinutes = Math.Max(5, _options.Value.ScanIntervalMinutes);
        var delay = TimeSpan.FromMinutes(scanIntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loan notifications scan failed");
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ProcessOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var now = DateTime.UtcNow;
        var today = now.Date;

        await SendOverdueRemindersAsync(context, emailSender, now, cancellationToken);
        await SendPickupRemindersAsync(context, emailSender, today, cancellationToken);
    }

    private async Task SendOverdueRemindersAsync(
        ApplicationDbContext context,
        IEmailSender emailSender,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var overdueLoans = await context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Where(l => l.Status == LoanStatus.Borrowed
                        && l.DueDate.HasValue
                        && l.DueDate.Value < nowUtc
                        && l.OverdueReminderSentAt == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);

        if (overdueLoans.Count == 0)
        {
            return;
        }

        foreach (var loan in overdueLoans)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = loan.User;
            var book = loan.Book;

            if (user == null || !user.IsActive)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !user.EmailConfirmed)
            {
                continue;
            }

            if (book == null)
            {
                continue;
            }

            var due = loan.DueDate!.Value.ToString("yyyy-MM-dd");
            var subject = "Przypomnienie: przekroczony termin zwrotu";
            var body = $"<p>Termin zwrotu książki <strong>{System.Net.WebUtility.HtmlEncode(book.Title)}</strong> minął: <strong>{due}</strong>.</p>" +
                       $"<p>Prosimy o jak najszybszy zwrot lub kontakt z biblioteką.</p>";

            try
            {
                loan.OverdueReminderSentAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);

                await emailSender.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send overdue reminder for LoanId={LoanId}", loan.Id);
            }
        }
    }

    private async Task SendPickupRemindersAsync(
        ApplicationDbContext context,
        IEmailSender emailSender,
        DateTime todayUtcDate,
        CancellationToken cancellationToken)
    {
        var daysBefore = _options.Value.PickupReminderDaysBefore;
        if (daysBefore < 0)
        {
            daysBefore = 0;
        }

        var targetDate = todayUtcDate.AddDays(daysBefore);

        var pickupLoans = await context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Where(l => l.Status == LoanStatus.OnHold
                        && l.DueDate.HasValue
                        && l.DueDate.Value.Date == targetDate
                        && l.PickupReminderSentAt == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);

        if (pickupLoans.Count == 0)
        {
            return;
        }

        foreach (var loan in pickupLoans)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = loan.User;
            var book = loan.Book;

            if (user == null || !user.IsActive)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !user.EmailConfirmed)
            {
                continue;
            }

            if (book == null)
            {
                continue;
            }

            var due = loan.DueDate!.Value.ToString("yyyy-MM-dd");
            var subject = "Przypomnienie: odbiór zarezerwowanej książki";
            var body = $"<p>Przypomnienie o odbiorze książki <strong>{System.Net.WebUtility.HtmlEncode(book.Title)}</strong>.</p>" +
                       $"<p>Termin odbioru: <strong>{due}</strong>.</p>";

            try
            {
                loan.PickupReminderSentAt = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);

                await emailSender.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send pickup reminder for LoanId={LoanId}", loan.Id);
            }
        }
    }
}
