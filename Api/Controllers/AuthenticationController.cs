using System.ComponentModel.DataAnnotations;
using Api.DTO;
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
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        ApplicationDbContext context,
        ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists.");
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Creating a new user entity
        var newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = passwordHash
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok("Registration successful.");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == request.Username && u.Password == request.Password);

        if (user == null) return Unauthorized("Invalid username or password.");

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, request.Password);
        if (!valid) return Unauthorized("Invalid username or password.");

        return Ok(new { Message = "Login successful", UserId = user.UserId });
    }
}
