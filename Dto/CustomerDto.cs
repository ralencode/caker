using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record CustomerResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("email")] string Email
    );
}
