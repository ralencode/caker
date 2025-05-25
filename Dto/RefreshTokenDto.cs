using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record RefreshTokenResponse(
        [property: JsonPropertyName("user")] UserResponse User,
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("expires")] DateTime Expires,
        [property: JsonPropertyName("revoked")] DateTime? Revoked,
        [property: JsonPropertyName("is_expired")] bool IsExpired,
        [property: JsonPropertyName("is_active")] bool IsActive
    );
}
