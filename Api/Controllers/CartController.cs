using Application.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }
    
    private int GetCurrentUserId()
    {
        if (HttpContext.Items["UserId"] is int userId)
            return userId;
        
        throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            if (HttpContext.Items["IsAuthenticated"] as bool? != true)
            {
                return Unauthorized(new { message = "Not authenticated" });
            }

            int userId = GetCurrentUserId();

            var cart = await _cartService.GetUserCart(userId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart");
            return StatusCode(500, new { message = "An error occurred while retrieving the cart" });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO addToCartDTO)
    {
        try
        {
            if (HttpContext.Items["IsAuthenticated"] as bool? != true)
            {
                return Unauthorized(new { message = "Not authenticated" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = GetCurrentUserId();

            var cart = await _cartService.AddToCart(userId, addToCartDTO);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, new { message = "An error occurred while adding to cart" });
        }
    }

    [HttpPut("update/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDTO updateDTO)
    {
        try
        {
            if (HttpContext.Items["IsAuthenticated"] as bool? != true)
            {
                return Unauthorized(new { message = "Not authenticated" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = GetCurrentUserId();

            var cart = await _cartService.UpdateCartItem(userId, cartItemId, updateDTO);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item");
            return StatusCode(500, new { message = "An error occurred while updating cart item" });
        }
    }

    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        try
        {
            if (HttpContext.Items["IsAuthenticated"] as bool? != true)
            {
                return Unauthorized(new { message = "Not authenticated" });
            }

            int userId = GetCurrentUserId();

            var cart = await _cartService.RemoveFromCart(userId, cartItemId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
            return StatusCode(500, new { message = "An error occurred while removing item" });
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            if (HttpContext.Items["IsAuthenticated"] as bool? != true)
            {
                return Unauthorized(new { message = "Not authenticated" });
            }

            int userId = GetCurrentUserId();

            await _cartService.ClearCart(userId);
            return Ok(new { message = "Cart cleared successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, new { message = "An error occurred while clearing cart" });
        }
    }
}