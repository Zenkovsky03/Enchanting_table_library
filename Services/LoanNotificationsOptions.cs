namespace Biblioteka.Services;

public sealed class LoanNotificationsOptions
{
    public int PickupReminderDaysBefore { get; set; } = 1;

    public int ScanIntervalMinutes { get; set; } = 60;
}
