namespace Domain.CartItems;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Carts;
using Domain.Products;

public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    [Required]
    [ForeignKey(nameof(Cart))]
    public int CartId { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    
    [Required]
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Cart Cart { get; set; }
    public virtual Product Product { get; set; }
}