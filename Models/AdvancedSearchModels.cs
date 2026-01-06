using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models;

public enum SearchMode
{
    All = 0,  // AND - wszystkie kryteria muszą być spełnione
    Any = 1,  // OR - dowolne kryterium
}

public class AdvancedSearchQuery
{
    // Kryteria wyszukiwania (szukaj)
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Isbn { get; set; }

    // Wykluczenia (nie zawiera)
    public string? ExcludeTitle { get; set; }
    public string? ExcludeAuthor { get; set; }

    // Tryb wyszukiwania
    public SearchMode Mode { get; set; } = SearchMode.All;

    public int? CategoryId { get; set; }

    public int[] TagIds { get; set; } = Array.Empty<int>();

    public bool HasAnyCriteria()
    {
        return !string.IsNullOrWhiteSpace(Title)
               || !string.IsNullOrWhiteSpace(Author)
               || !string.IsNullOrWhiteSpace(Isbn)
               || !string.IsNullOrWhiteSpace(ExcludeTitle)
               || !string.IsNullOrWhiteSpace(ExcludeAuthor)
               || CategoryId.HasValue
               || (TagIds != null && TagIds.Length > 0);
    }

    public bool HasExclusions => !string.IsNullOrWhiteSpace(ExcludeTitle) || !string.IsNullOrWhiteSpace(ExcludeAuthor);
}

public class AdvancedSearchViewModel
{
    public AdvancedSearchQuery Query { get; set; } = new();

    public List<Category> Categories { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    public List<Book> Results { get; set; } = new();

    public int CartCount { get; set; }

    public bool HasSearched { get; set; }
}
