using Application.DTO;
using Domain;
using Domain.OrderItems;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrderDTO> CreateOrder(int userId, CreateOrderDTO createOrderDTO)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    throw new KeyNotFoundException("Cart not found");
                }

                if (!cart.CartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                decimal totalAmount = 0;
                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Product.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for product: {cartItem.Product.ProductName}");
                    }

                    totalAmount += cartItem.Product.Price * cartItem.Quantity;
                }

                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Pending,
                    ShippingAddress = createOrderDTO.ShippingAddress,
                    PaymentMethod = createOrderDTO.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = cartItem.Product.Price
                    };

                    _context.OrderItems.Add(orderItem);

                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    cartItem.Product.UpdatedAt = DateTime.UtcNow;
                }

                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} created for user {UserId}", order.OrderId, userId);

                return await GetOrderDetails(userId, order.OrderId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                throw;
            }
    }
    public async Task<List<OrderDTO>> GetUserOrders(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToOrderDTO).ToList();
        }

        public async Task<OrderDTO> GetOrderDetails(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            return MapToOrderDTO(order);
        }

        public async Task<OrderDTO> CancelOrder(int userId, int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

                if (order == null)
                {
                    throw new KeyNotFoundException("Order not found");
                }

                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    throw new InvalidOperationException("Order is already cancelled");
                }

                if (order.OrderStatus == OrderStatus.Delivered)
                {
                    throw new InvalidOperationException("Cannot cancel delivered orders");
                }

                order.OrderStatus = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product.StockQuantity += orderItem.Quantity;
                    orderItem.Product.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} cancelled for user {UserId}", orderId, userId);

                return MapToOrderDTO(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                throw;
            }
        }

        private OrderDTO MapToOrderDTO(Order order)
        {
            return new OrderDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus.ToString(),
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductName,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    Subtotal = oi.PriceAtPurchase * oi.Quantity
                }).ToList()
        };
    }
}