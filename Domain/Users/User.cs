using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Carts;
using Domain.Orders;

namespace Domain.Users;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required]
    public string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    [JsonIgnore]
    public string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Cart? Cart { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}