using System;
using System.ComponentModel.DataAnnotations;

public class SavedSearch
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }  // np. "ASP.NET książki"

    // Tu możesz zapisać parametry wyszukiwania w formie JSON/QueryString
    [Required]
    public string QueryData { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}