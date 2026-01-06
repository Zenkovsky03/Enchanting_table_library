namespace Biblioteka.Areas.Admin.Models;

public class AdminLoansIndexViewModel
{
    public string? Q { get; set; }
    public int? Status { get; set; }
    public bool OverdueOnly { get; set; }

    public List<Loan> Loans { get; set; } = new();
}
