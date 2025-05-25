using System.Security.Claims;
using Caker.Models;

namespace Caker.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> GenerateRefreshTokenAsync(User user);

        Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
