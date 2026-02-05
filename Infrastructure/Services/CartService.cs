using Application.DTO;
using Domain;
using Domain.CartItems;
using Domain.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CartService> _logger;

    public CartService(ApplicationDbContext context, ILogger<CartService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CartDTO> GetUserCart(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            throw new Exception("Cart not found");
        }

        return MapToCartDTO(cart);
    }

    public async Task<CartDTO> AddToCart(int userId, AddToCartDTO addToCartDTO)
    {
        var product = await _context.Products.FindAsync(addToCartDTO.ProductId);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        if (product.StockQuantity < addToCartDTO.Quantity)
        {
            throw new InvalidOperationException("Insufficient stock available");
        }
        
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found for user");
        }
        
        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDTO.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += addToCartDTO.Quantity;
            
            if (product.StockQuantity < existingItem.Quantity)
            {
                throw new InvalidOperationException("Insufficient stock for requested quantity");
            }
        }
        else
        {
            var newCartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = addToCartDTO.ProductId,
                Quantity = addToCartDTO.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            cart.CartItems.Add(newCartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product {ProductId} added to cart for user {UserId}", 
            addToCartDTO.ProductId, userId);

        return await GetUserCart(userId);
    }
    
    public async Task<CartDTO> UpdateCartItem(int userId, int cartItemId, UpdateCartItemDTO updateDto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            throw new KeyNotFoundException("Cart not found");
        }

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

        if (cartItem == null)
        {
            throw new KeyNotFoundException("Cart item not found");
        }
        
        if (updateDto.Quantity == 0)
        {
            cart.CartItems.Remove(cartItem);
        }
        else
        {
            if (cartItem.Product.StockQuantity < updateDto.Quantity)
            {
                throw new InvalidOperationException("Insufficient stock available");
            }

            cartItem.Quantity = updateDto.Quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cart item {CartItemId} updated for user {UserId}", cartItemId, userId);

        return await GetUserCart(userId);
    }
    
     public async Task<CartDTO> RemoveFromCart(int userId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw new KeyNotFoundException("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                throw new KeyNotFoundException("Cart item not found");
            }
            
            cart.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cart item {CartItemId} removed for user {UserId}", cartItemId, userId);

            return await GetUserCart(userId);
        }

        public async Task<bool> ClearCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw new KeyNotFoundException("Cart not found");
            }
            
            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cart cleared for user {UserId}", userId);

            return true;
        }

        private CartDTO MapToCartDTO(Cart cart)
        {
            var cartDTO = new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemDTO
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.ProductName,
                    Description = ci.Product.Description,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity
                }).ToList()
            };

            cartDTO.TotalPrice = cartDTO.Items.Sum(i => i.Subtotal);
            cartDTO.TotalItems = cartDTO.Items.Sum(i => i.Quantity);

            return cartDTO;
        }
}