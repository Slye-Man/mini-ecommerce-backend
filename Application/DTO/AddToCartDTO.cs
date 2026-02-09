using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class AddToCartDTO
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
    public int Quantity { get; set; }
}