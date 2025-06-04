using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record SearchQuery([property: JsonPropertyName("name")] string Name);
}
