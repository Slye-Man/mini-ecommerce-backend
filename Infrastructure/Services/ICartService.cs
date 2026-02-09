using Application.DTO;

namespace Infrastructure.Services;

public interface ICartService
{
    Task<CartDTO> GetUserCart(int userId);
    Task<CartDTO> AddToCart(int userId, AddToCartDTO addToCartDTO);
    Task<CartDTO> UpdateCartItem(int userId, int cartItemId, UpdateCartItemDTO updateDTO);
    Task<CartDTO> RemoveFromCart(int userId, int cartItemId);
    Task<bool> ClearCart(int userId);
}