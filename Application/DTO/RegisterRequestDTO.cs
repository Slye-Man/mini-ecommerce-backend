using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class RegisterRequestDTO
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3)]
    public required string UserName { get; set; }
    
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }
    
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public required string Password { get; set; }
    
    
    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}