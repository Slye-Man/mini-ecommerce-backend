using System.ComponentModel.DataAnnotations;
using Domain.Users;

namespace Domain;

public class Session
{
    [Key]
    public int SessionId { get; set; }

    [Required]
    [StringLength(200)]
    public string? SessionToken { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; }
    
    
    public virtual User? User { get; set; }
}