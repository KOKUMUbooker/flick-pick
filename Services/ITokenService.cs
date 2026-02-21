using FlickPickApp.Models;
namespace FlickPickApp.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, RoleEnum role, out string jwtId, Client client);
    RefreshToken GenerateRefreshToken(string ipAddress, string jwtId, Client client, Guid userId);
}
