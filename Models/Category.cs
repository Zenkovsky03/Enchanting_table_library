using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int? ParentCategoryId { get; set; }

    [ValidateNever]
    public Category? ParentCategory { get; set; } 

    [ValidateNever]
    public ICollection<Category> Children { get; set; } = new List<Category>();

    [ValidateNever]
    public ICollection<Book> Books { get; set; } = new List<Book>();
}


public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public ICollection<BookTag> BookTags { get; set; }
}

// Tabela łącząca Book–Tag (wiele-do-wielu)
public class BookTag
{
    public int BookId { get; set; }
    public Book Book { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}
