using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

public enum LoanStatus
{
    InStock = 0,   // książka w magazynie (jeszcze nie na półce czytelnika)
    OnHold = 1,    // oczekuje na odbiór (na półce czytelnika)
    Borrowed = 2,  // wypożyczona
    Returned = 3,  // zwrócona
    Cancelled = 4  // anulowana rezerwacja
}

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }
    public Book Book { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.InStock;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // złożenie zamówienia
    public DateTime? BorrowedAt { get; set; }                  // faktyczne wypożyczenie
    public DateTime? ReturnedAt { get; set; }                  // zwrot

    // Możesz wykorzystać do filtrowania archiwum lub przyszłej logiki terminów
    public DateTime? DueDate { get; set; }
}