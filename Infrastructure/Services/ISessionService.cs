namespace Infrastructure.Services;

public interface ISessionService
{
    Task<string> CreateSession(int userId);
    Task<int?> GetUserIdFromSession(string sessionToken);
    Task<bool> ValidateSession(string sessionToken);
    Task<bool> DeleteSession(string sessionToken);
    Task CleanupExpiredSessions();
}