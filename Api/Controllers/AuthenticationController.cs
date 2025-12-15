using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthenticationController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return BadRequest("Email already exists.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("Registration successful.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == request.Username && u.Password == request.Password);

        if (user == null) return Unauthorized("Invalid username or password.");

        return Ok(new { Message = "Login successful", UserId = user.UserId });
    }
}

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Password { get; set; }
}
