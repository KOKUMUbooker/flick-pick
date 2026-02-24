using WatchHive.Models;
namespace WatchHive.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, RoleEnum role, out string jwtId);
    RefreshToken GenerateRefreshToken(string ipAddress, string jwtId, Guid userId);
}
