using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class CreateOrderDTO
{
    [Required(ErrorMessage = "Shipping address is required")]
    [StringLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters")]
    public string ShippingAddress { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [StringLength(100, ErrorMessage = "Payment method cannot exceed 100 characters")]
    public string PaymentMethod { get; set; }
}