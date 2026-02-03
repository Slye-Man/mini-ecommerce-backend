using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateProfileDTO
{
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
    [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters")]
    public string? Username { get; set; }
}