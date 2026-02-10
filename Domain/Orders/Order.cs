using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.OrderItems;
using Domain.Users;

namespace Domain.Orders;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Required]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string ShippingAddress { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}