using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Caker.Services;

public class JwtCookieHandler(
    IOptionsMonitor<JwtBearerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration config
) : AuthenticationHandler<JwtBearerOptions>(options, logger, encoder)
{
    private readonly IConfiguration _config = config;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Cookies["accessToken"];
        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var principal = tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out _
            );

            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }
    }
}
