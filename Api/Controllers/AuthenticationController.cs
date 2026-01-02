using System.ComponentModel.DataAnnotations;
using Api.DTO;
using Domain;
using Domain.Carts;
using Domain.Users;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IAuthService _authService;

    public AuthenticationController(
        ApplicationDbContext context,
        ILogger<AuthenticationController> logger,
        IAuthService authService)
    {
        _context = context;
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        try 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResponse = await _authService.RegisterUser(request);
            return Ok(authResponse);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _authService.LoginUser(request);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };

            if (!string.IsNullOrEmpty(user.SessionId))
            {
                Response.Cookies.Append("SessionId", user.SessionId, cookieOptions);
            }
            
            return Ok(new { Message = "Login successful", UserId = user.UserId });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid username or password.");
        }

    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Get session ID from cookie
            var sessionId = HttpContext.Items["sessionId"]?.ToString();

            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest(new { message = "No active session found" });
            }

            // Remove session (CSRF VULNERABLE - no token validation)
            await _authService.LogoutUser(sessionId);

            // Delete cookie
            Response.Cookies.Delete("sessionId");
            Response.Cookies.Delete("SessionId");

            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in logout endpoint");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(user);
    }
}