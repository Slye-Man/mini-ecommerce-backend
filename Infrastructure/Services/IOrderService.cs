using Application.DTO;

namespace Infrastructure.Services;

public interface IOrderService
{
    Task<OrderDTO> CreateOrder(int userId, CreateOrderDTO createOrderDTO);
    Task<List<OrderDTO>> GetUserOrders(int userId);
    Task<OrderDTO> GetOrderDetails(int userId, int orderId);
    Task<OrderDTO> CancelOrder(int userId, int orderId);
}