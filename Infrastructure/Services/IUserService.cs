using Application.DTO;

namespace Infrastructure.Services;

public interface IUserService
{
    Task<UserProfileDTO> GetUserProfile(int userId);
    Task<UserProfileDTO> UpdateUserProfile(int userId, UpdateProfileDTO updateDTO);
    Task<bool> ChangePassword(int userId, ChangePasswordDTO passwordDTO);
    Task<bool> DeleteAccount(int userId);
}