using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Orders;
using Domain.Products;

namespace Domain.OrderItems;

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtPurchase { get; set; }

    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
}