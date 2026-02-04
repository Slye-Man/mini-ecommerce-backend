using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class ChangePasswordDTO
{
    [Required(ErrorMessage = "Current password is required")]
    public string? OldPassword { get; set; }
    
    [Required(ErrorMessage = "New password is required")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    public string? NewPassword { get; set; }
    
    [Required(ErrorMessage = "Confirm new password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string? ConfirmNewPassword { get; set; }
}