using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record CustomerResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("user_id")] int UserId
    );
}
