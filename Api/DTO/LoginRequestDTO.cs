using System.ComponentModel.DataAnnotations;

namespace Api.DTO;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "Username is required")]
    public required string Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}