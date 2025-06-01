using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record CreateRegularCakeRequest(
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("description")] string? description,
        [property: JsonPropertyName("diameter")] double? diameter,
        [property: JsonPropertyName("weight")] double? weight,
        [property: JsonPropertyName("required_time")] int? required_time,
        [property: JsonPropertyName("price")] int price
    ) : ImageUpload;

    public record CreateCustomCakeRequest(
        [property: JsonPropertyName("confectioner_id")] int confectioner_id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("description")] string? description,
        [property: JsonPropertyName("fillings")] ICollection<string> fillings,
        [property: JsonPropertyName("required_time")] int? required_time,
        [property: JsonPropertyName("color")] string? color,
        [property: JsonPropertyName("diameter")] double? diameter,
        [property: JsonPropertyName("text")] string? text,
        [property: JsonPropertyName("text_size")] double? text_size,
        [property: JsonPropertyName("text_x")] double? text_x,
        [property: JsonPropertyName("text_y")] double? text_y,
        [property: JsonPropertyName("price")] int? price,
        [property: JsonPropertyName("image_scale")] double image_scale
    ) : ImageUpload;

    public record UpdateCustomCakeRequest(
        [property: JsonPropertyName("confectioner_id")] int? confectioner_id,
        [property: JsonPropertyName("name")] string? name,
        [property: JsonPropertyName("description")] string? description,
        [property: JsonPropertyName("fillings")] ICollection<string>? fillings,
        [property: JsonPropertyName("required_time")] int? required_time,
        [property: JsonPropertyName("color")] string? color,
        [property: JsonPropertyName("diameter")] double? diameter,
        [property: JsonPropertyName("text")] string? text,
        [property: JsonPropertyName("text_size")] double? text_size,
        [property: JsonPropertyName("text_x")] double? text_x,
        [property: JsonPropertyName("text_y")] double? text_y,
        [property: JsonPropertyName("price")] int? price,
        [property: JsonPropertyName("image_scale")] double? image_scale
    ) : ImageUpload;

    public record CakeResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("confectioner")] ConfectionerResponse? Confectioner,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("fillings")] ICollection<string>? Fillings,
        [property: JsonPropertyName("required_time")] int? ReqTime,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("image_url")] string ImageUrl,
        [property: JsonPropertyName("price")] int? Price,
        [property: JsonPropertyName("diameter")] double? Diameter,
        [property: JsonPropertyName("weight")] double? Weight,
        [property: JsonPropertyName("text")] string? Text,
        [property: JsonPropertyName("text_size")] double? TextSize,
        [property: JsonPropertyName("text_x")] double? TextX,
        [property: JsonPropertyName("text_y")] double? TextY,
        [property: JsonPropertyName("is_custom")] bool IsCustom,
        [property: JsonPropertyName("image_scale")] double? ImageScale
    );
}
