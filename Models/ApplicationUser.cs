using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    // Czy konto czytelnika jest zatwierdzone przez admina
    public bool IsApproved { get; set; } = false;

    // Czy konto jest aktywne (możesz tym blokować wypożyczenia)
    public bool IsActive { get; set; } = true;

    public ICollection<Loan> Loans { get; set; }
    public ICollection<SavedSearch> SavedSearches { get; set; }
}
