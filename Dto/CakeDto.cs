using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record CreateRegularCakeRequest(
        [property: JsonPropertyName("confectioner_id")] int ConfectionerId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("diameter")] double Diameter,
        [property: JsonPropertyName("weight")] double Weight,
        [property: JsonPropertyName("eta_days")] int ETADays,
        [property: JsonPropertyName("price")] int Price
    ) : ImageUpload;

    public record CreateCustomCakeRequest(
        [property: JsonPropertyName("confectioner_id")] int ConfectionerId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("eta_days")] int ETADays,
        [property: JsonPropertyName("color")] long Color,
        [property: JsonPropertyName("diameter")] double Diameter,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("text_size")] double TextSize,
        [property: JsonPropertyName("text_x")] double TextX,
        [property: JsonPropertyName("text_y")] double TextY,
        [property: JsonPropertyName("price")] int? Price
    ) : ImageUpload;

    public record UpdateCustomCakeRequest(
        [property: JsonPropertyName("confectioner_id")] int? ConfectionerId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("fillings")] ICollection<string>? Fillings,
        [property: JsonPropertyName("eta_days")] int? ETADays,
        [property: JsonPropertyName("color")] long? Color,
        [property: JsonPropertyName("diameter")] double? Diameter,
        [property: JsonPropertyName("text")] string? Text,
        [property: JsonPropertyName("text_size")] double? TextSize,
        [property: JsonPropertyName("text_x")] double? TextX,
        [property: JsonPropertyName("text_y")] double? TextY,
        [property: JsonPropertyName("price")] int? Price
    ) : ImageUpload;

    public record CakeResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("image_url")] string ImageUrl,
        [property: JsonPropertyName("price")] int Price,
        [property: JsonPropertyName("diameter")] double? Diameter,
        [property: JsonPropertyName("weight")] double? Weight,
        [property: JsonPropertyName("text")] string? Text,
        [property: JsonPropertyName("text_size")] double? TextSize,
        [property: JsonPropertyName("text_x")] double? TextX,
        [property: JsonPropertyName("text_y")] double? TextY,
        [property: JsonPropertyName("is_custom")] bool IsCustom
    );
}
