namespace Biblioteka.Models;

public class BooksBrowseViewModel
{
    public List<Book> Books { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    public string? Q { get; set; }
    public int? CategoryId { get; set; }
    public int? TagId { get; set; }

    public int CartCount { get; set; }
}
