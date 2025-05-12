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
}
