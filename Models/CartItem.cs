using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models;

public class CartItem
{
    [Range(1, int.MaxValue)]
    public int BookId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;
}
