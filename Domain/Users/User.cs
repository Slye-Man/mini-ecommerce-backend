using System.ComponentModel.DataAnnotations;

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
    public string Password  { get; set; }
}