using Api.DTO;
using Domain.Users;

namespace Infrastructure.Services;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO register);
    Task<AuthResponseDTO> LoginUser(LoginRequestDTO login);
    Task<bool> ValidateSession(string sessionId);
    Task<User> GetUserBySession(string sessionId);
    Task<bool> LogoutUser(string sessionId);
}