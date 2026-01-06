namespace Biblioteka.Models;

public class HomeIndexViewModel
{
    public List<News> News { get; set; } = new();
    public List<Book> NewBooks { get; set; } = new();
}
