using FlickPickApp.DTOs;
using FlickPickApp.Models;

namespace FlickPickApp.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null);
    Task<AuthResult> AuthenticateUserAsync(UserLoginDto loginDto, string ipAddress);
    Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken, string clientId, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
    string GenerateVerificationToken();
}
