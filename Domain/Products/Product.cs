using System.ComponentModel.DataAnnotations;

namespace Domain.Products;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    public string? ProductName { get; set; }   
    
    public decimal Price { get; set; }
}