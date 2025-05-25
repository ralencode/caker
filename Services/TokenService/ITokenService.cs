using System.Security.Claims;
using Caker.Models;

namespace Caker.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
