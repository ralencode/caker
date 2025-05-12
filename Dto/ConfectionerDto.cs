using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public record ConfectionerSettingsResponse(
        [property: JsonPropertyName("confectioner_id")] int? ConfectionerId,
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinETADays,
        [property: JsonPropertyName("max_eta_days")] int MaxETADays,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes
    );

    public record UpdateConfectionerSettingsRequest(
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinETADays,
        [property: JsonPropertyName("max_eta_days")] int MaxETADays,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes
    );

    public record CreateConfectionerRequest(
        [property: JsonPropertyName("user_id")] int UserId,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("rating")] double Rating,
        [property: JsonPropertyName("address")] string Address,
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinEta,
        [property: JsonPropertyName("max_eta_days")] int MaxEta,
        [property: JsonPropertyName("fillings")] ICollection<string>? Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes
    );

    public record UpdateConfectionerRequest(
        [property: JsonPropertyName("user_id")] int? UserId,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("rating")] double? Rating,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("min_diameter")] double? MinDiameter,
        [property: JsonPropertyName("max_diameter")] double? MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int? MinEta,
        [property: JsonPropertyName("max_eta_days")] int? MaxEta,
        [property: JsonPropertyName("fillings")] ICollection<string>? Fillings,
        [property: JsonPropertyName("do_images")] bool? DoImages,
        [property: JsonPropertyName("do_shapes")] bool? DoShapes
    );

    public record ConfectionerResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("user_id")] int UserId,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("rating")] double Rating,
        [property: JsonPropertyName("address")] string Address,
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinEta,
        [property: JsonPropertyName("max_eta_days")] int MaxEta,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes
    );
}
