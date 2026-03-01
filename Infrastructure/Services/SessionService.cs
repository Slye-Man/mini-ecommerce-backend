using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionService> _logger;
    
    public SessionService(ApplicationDbContext context, ILogger<SessionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> CreateSession(int userId)
    {
        string sessionToken = Guid.NewGuid().ToString();

        var session = new Session
        {
            SessionToken = sessionToken,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Session created for user {UserId}", userId);
        
        return sessionToken;
    }

    public async Task<int?> GetUserIdFromSession(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
            return null;

        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

        if (session == null)
            return null;

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return null;
        }

        return session.UserId;
    }
    
    public async Task<bool> ValidateSession(string sessionToken)
    {
        var userId = await GetUserIdFromSession(sessionToken);
        return userId.HasValue;
    }

    public async Task<bool> DeleteSession(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
            return false;

        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

        if (session == null)
            return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Session deleted for user {UserId}", session.UserId);

        return true;
    }

    public async Task CleanupExpiredSessions()
    {
        var expiredSessions = await _context.Sessions
            .Where(s => s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredSessions.Any())
        {
            _context.Sessions.RemoveRange(expiredSessions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);
        }
    }
}