using System.Collections.Concurrent;
using Api.DTO;
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

    private static readonly ConcurrentDictionary<string, SessionData> _sessions = new();
    
    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
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
           Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
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

        // Create session
        var sessionId = Guid.NewGuid().ToString();

        var sessionData = new SessionData
        {
            UserId = user.UserId,
            ExpiresAt = DateTime.UtcNow.AddHours(1) // Session valid for 1 hour
        };
        
        _sessions[sessionId] = sessionData;

        _logger.LogInformation("User logged in: {Username}", user.UserName);

        return new AuthResponseDTO()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            SessionId = sessionId
        };
    }

    public Task<bool> ValidateSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            return Task.FromResult(false);

        if (!_sessions.TryGetValue(sessionId, out var session))
            return Task.FromResult(false);

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            _sessions.TryRemove(sessionId, out _);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(true);
    }

    public Task<User> GetUserBySession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId) || !_sessions.TryGetValue(sessionId, out var session))
            return Task.FromResult<User>(null);
        
        if (session.ExpiresAt < DateTime.UtcNow)
        {
            _sessions.TryRemove(sessionId, out _);
            return Task.FromResult<User>(null);
        }
        
        //return user from database
        return _context.Users.FirstOrDefaultAsync(u => u.UserId == session.UserId);
    }

    public Task<bool> LogoutUser(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            return Task.FromResult(false);
        
        var removed = _sessions.TryRemove(sessionId, out _);
        
        if (removed)
            _logger.LogInformation("User logged out, sessionId: {SessionId}", sessionId);
        
        return Task.FromResult(removed);
    }

    private class SessionData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        
    }
}