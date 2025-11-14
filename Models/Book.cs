using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(150)]
    public string Author { get; set; }

    [StringLength(20)]
    public string Isbn { get; set; }

    // Opis książki
    public string Description { get; set; }

    // Wyciąg ze spisu treści
    public string TableOfContentsExcerpt { get; set; }

    // Nowości na stronie głównej
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    public bool IsNew { get; set; } = false;

    // Stany magazynowe
    public int StockCount { get; set; }

    // Kategoria (dokładnie jedna)
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // Tagi (wiele-do-wielu)
    public ICollection<BookTag> BookTags { get; set; }

    // Dodatkowe pliki
    public ICollection<BookFile> Files { get; set; }

    public ICollection<Loan> Loans { get; set; }
}

public class BookFile
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }
    public Book Book { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; }

    // Ścieżka/pliki w wwwroot/uploads itp.
    [Required]
    [StringLength(400)]
    public string FilePath { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
