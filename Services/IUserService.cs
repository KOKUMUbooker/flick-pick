using WatchHive.DTOs;
using WatchHive.Models;

namespace WatchHive.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null);
    Task<AuthResult> AuthenticateUserAsync(UserLoginDto loginDto, string ipAddress);
    Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken, string clientId, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
    string GenerateVerificationToken();
}
