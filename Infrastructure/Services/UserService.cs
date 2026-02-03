using Application.DTO;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;
    
    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfileDTO> GetUserProfile(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return new UserProfileDTO
        {
            UserId = user.UserId,
            Username = user.UserName,
            Email = user.Email
        };
    }
    
    public async Task<UserProfileDTO> UpdateUserProfile(int userId, UpdateProfileDTO updateDTO)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException("User not found");
        
        user.UserName = updateDTO.Username ?? user.UserName;
        user.Email = updateDTO.Email ?? user.Email;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User profile updated: {UserId}", user.UserId);
        
        return await GetUserProfile(userId);
    }

    public async Task<bool> ChangePassword(int userId, ChangePasswordDTO passwordDTO)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(passwordDTO.OldPassword, user.Password))
            throw new UnauthorizedAccessException("Incorrect Password");

        user.Password = BCrypt.Net.BCrypt.HashPassword(passwordDTO.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Password change successful: {UserId}", userId);
        
        return true;
    }

    public async Task<bool> DeleteAccount(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Cart)
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User account deleted: {UserId}", userId);
        
        return true;
    }
}