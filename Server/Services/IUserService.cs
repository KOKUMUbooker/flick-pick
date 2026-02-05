using MovieManager.DTOs;
using MovieManager.Models;

namespace MovieManager.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null);
    Task<AuthResponseDTO?> AuthenticateUserAsync(UserLoginDto loginDto, string ipAddress);
    Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken, string clientId, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
}