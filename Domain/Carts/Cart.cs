using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Users;
using Domain.CartItems;

namespace Domain.Carts;

public class Cart
{
    [Key]
    public int CartId { get; set; }
    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}