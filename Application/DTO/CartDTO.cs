namespace Application.DTO;

public class CartDTO
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public List<CartItemDTO> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}

public class CartItemDTO
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}