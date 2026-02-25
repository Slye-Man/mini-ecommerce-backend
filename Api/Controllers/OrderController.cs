using Application.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
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

                // TODO: Get actual userId from session
                int userId = 1;

                var order = await _orderService.CreateOrder(userId, createOrderDTO);
                return Ok(order);
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
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                if (HttpContext.Items["IsAuthenticated"] as bool? != true)
                {
                    return Unauthorized(new { message = "Not authenticated" });
                }

                // TODO: Get actual userId from session
                int userId = 1;

                var orders = await _orderService.GetUserOrders(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "An error occurred while retrieving orders" });
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            try
            {
                if (HttpContext.Items["IsAuthenticated"] as bool? != true)
                {
                    return Unauthorized(new { message = "Not authenticated" });
                }

                // TODO: Get actual userId from session
                int userId = 1;

                var order = await _orderService.GetOrderDetails(userId, orderId);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details");
                return StatusCode(500, new { message = "An error occurred while retrieving order details" });
            }
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                if (HttpContext.Items["IsAuthenticated"] as bool? != true)
                {
                    return Unauthorized(new { message = "Not authenticated" });
                }

                // TODO: Get actual userId from session
                int userId = 1;

                var order = await _orderService.CancelOrder(userId, orderId);
                return Ok(order);
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
                _logger.LogError(ex, "Error cancelling order");
                return StatusCode(500, new { message = "An error occurred while cancelling the order" });
            }
        }
    }