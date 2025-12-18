using System.ComponentModel.DataAnnotations;

namespace Domain.Carts;

public class Cart
{
    [Key]
    public int CartId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
}