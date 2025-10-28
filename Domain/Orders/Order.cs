using System.ComponentModel.DataAnnotations;

namespace Domain.Orders;

public class Order
{
    [Key]
    public int OrderId { get; set; }
}