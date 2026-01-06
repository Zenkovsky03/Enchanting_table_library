using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

public enum LoanStatus
{
    InStock = 0,
    OnHold = 1,
    Borrowed = 2,
    Returned = 3,
    Cancelled = 4,
    Waiting = 5
}

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [ValidateNever]
    public Book? Book { get; set; }

    [Required]
    public string UserId { get; set; }

    [ValidateNever]
    public ApplicationUser? User { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.InStock;

    [ValidateNever]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [ValidateNever]
    public DateTime? BorrowedAt { get; set; }
    [ValidateNever]
    public DateTime? ReturnedAt { get; set; }

    [ValidateNever]
    public DateTime? DueDate { get; set; }

    [ValidateNever]
    public DateTime? OverdueReminderSentAt { get; set; }

    [ValidateNever]
    public DateTime? PickupReminderSentAt { get; set; }

    [ValidateNever]
    public DateTime? AvailabilityEmailSentAt { get; set; }
}