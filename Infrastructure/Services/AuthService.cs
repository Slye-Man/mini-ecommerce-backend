using System.Collections.Concurrent;
using Application.DTO;
using Domain;
using Domain.Carts;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;
    private readonly ISessionService _sessionService;
    
    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger,  ISessionService sessionService)
    {
        _context = context;
        _logger = logger;
        _sessionService = sessionService;
    }

    public async Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO register)
    {
       // Check if email already exists
       if (await _context.Users.AnyAsync(u => u.Email == register.Email))
       {
           throw new InvalidOperationException("Email is already registered");
       }
       
       // Check if username already exists
       if (await _context.Users.AnyAsync(u => u.UserName == register.UserName))
       {
           throw new InvalidOperationException("Username already taken");
       }
       
       string passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

       var newUser = new User
       {
           Email = register.Email,
           UserName = register.UserName,
           Password = passwordHash,
           CreatedAt = DateTime.UtcNow,
           UpdatedAt = DateTime.UtcNow
       };
       
       // Adding User to context
       _context.Users.Add(newUser);
       
       // Create empty cart
       var newCart = new Cart
       {
           User = newUser,
           Description = "Default Cart",
           CreatedAt = DateTime.UtcNow,
           UpdatedAt = DateTime.UtcNow
       };
       
       // Add cart to context
       _context.Carts.Add(newCart);
       
       //Saving changes to database
       await _context.SaveChangesAsync();
       
       _logger.LogInformation("New user registered: {Username}", newUser.UserName);

       return new AuthResponseDTO()
       {
           UserId = newUser.UserId,
           UserName = newUser.UserName,
           Email = newUser.Email
       };
    }

    public async Task<AuthResponseDTO> LoginUser(LoginRequestDTO login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == login.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // Create session in database
        var sessionId = await _sessionService.CreateSession(user.UserId);

        _logger.LogInformation("User logged in: {Username}", user.UserName);

        return new AuthResponseDTO()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            SessionId = sessionId
        };
    }

    public async Task<bool> ValidateSession(string sessionId)
    {
        return await _sessionService.ValidateSession(sessionId);
    }

    public async Task<User> GetUserBySession(string sessionId)
    {
        var userId = await _sessionService.GetUserIdFromSession(sessionId);
        
        if (!userId.HasValue)
            return null;
        
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> LogoutUser(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            return false;
        
        var removed = await _sessionService.DeleteSession(sessionId);
        
        if (removed)
            _logger.LogInformation("User logged out, sessionId: {SessionId}", sessionId);
        
        return removed;
    }
}