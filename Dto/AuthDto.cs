using System.Text.Json.Serialization;
using Caker.Models;

namespace Caker.Dto
{
    public record LoginRequest(
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("password")] string Password
    );

    public record RegisterRequest(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("password")] string Password,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("type")] UserType Type,
        [property: JsonPropertyName("description")] string? Description = null,
        [property: JsonPropertyName("address")] string? Address = null
    );

    public record RefreshTokenRequest([property: JsonPropertyName("token")] string Token);

    public record AuthResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("type")] UserType Type,
        [property: JsonPropertyName("customer")] CustomerResponse? Customer,
        [property: JsonPropertyName("confectioner")] ConfectionerResponse? Confectioner,
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken
    );

    public record RefreshResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken
    );
}
