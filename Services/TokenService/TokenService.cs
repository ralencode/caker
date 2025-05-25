using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Caker.Models;
using Caker.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Caker.Services.TokenService
{
    public class TokenService(IConfiguration config, RefreshTokenRepository refreshTokenRepo)
        : ITokenService
    {
        private readonly IConfiguration _config = config;
        private readonly RefreshTokenRepository _refreshTokenRepo = refreshTokenRepo;

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var token = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(
                    _config.GetValue<int>("Jwt:RefreshTokenExpiryDays")
                ),
            };
            await _refreshTokenRepo.Create(token);
            return token.Token;
        }

        public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepo.GetByToken(token);
            if (refreshToken == null || refreshToken.IsExpired || refreshToken.Revoked != null)
            {
                return null;
            }
            return refreshToken;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _config.GetValue<int>("Jwt:AccessTokenExpiryMinutes")
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
                ),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = false, // We check expiry manually
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(
                token,
                tokenValidationParameters,
                out var securityToken
            );

            if (
                securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
