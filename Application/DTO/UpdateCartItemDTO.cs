using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class UpdateCartItemDTO
{
    [Required]
    [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
    public int Quantity { get; set; }
}