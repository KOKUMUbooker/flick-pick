using MovieManager.DTOs;
using MovieManager.Models;

namespace MovieManager.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null);
    // Task<User> CreateUserAsync(RegisterUserDto userDto, Guid? roleId = null);
}