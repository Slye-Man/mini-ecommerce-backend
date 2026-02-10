namespace Application.DTO;

public class OrderDTO
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; }
    public string ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
}

public class OrderItemDTO
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public decimal Subtotal { get; set; }
}
