namespace Biblioteka.Models;

public class CartLineViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public int StockCount { get; set; }
    public int Quantity { get; set; }
}

public class CartIndexViewModel
{
    public List<CartLineViewModel> Lines { get; set; } = new();
    public int TotalItems => Lines.Sum(l => l.Quantity);
}

public class CheckoutResultViewModel
{
    public int CreatedLoans { get; set; }
}
