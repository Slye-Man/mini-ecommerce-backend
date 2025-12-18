using System.ComponentModel.DataAnnotations;
using Api.DTO;
using Domain;
using Domain.Carts;
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists.");

        if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
            return BadRequest("Username already exists.");
            
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Creating a new user entity with an associated cart (required navigation property)
        var newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = passwordHash,
            Cart = new Cart
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("New user registered: {Username}", newUser.UserName);
        
        // return Ok("Registration successful.");
        return CreatedAtAction(nameof(GetUser), new { id = newUser.UserId }, newUser);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == request.Username || u.Email == request.Username);

        if (user == null) return Unauthorized("Invalid username or password.");

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!valid) return Unauthorized("Invalid username or password.");

        return Ok(new { Message = "Login successful", UserId = user.UserId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(user);
    }
}