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
        [property: JsonPropertyName("do_shapes")] bool DoShapes,
        [property: JsonPropertyName("do_custom")] bool DoCustom
    );

    public record UpdateConfectionerSettingsRequest(
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinETADays,
        [property: JsonPropertyName("max_eta_days")] int MaxETADays,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes,
        [property: JsonPropertyName("do_custom")] bool DoCustom
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
        [property: JsonPropertyName("do_shapes")] bool DoShapes,
        [property: JsonPropertyName("do_custom")] bool DoCustom
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
        [property: JsonPropertyName("do_shapes")] bool? DoShapes,
        [property: JsonPropertyName("do_custom")] bool? DoCustom
    );

    public record ConfectionerResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("rating")] double Rating,
        [property: JsonPropertyName("address")] string Address,
        [property: JsonPropertyName("min_diameter")] double MinDiameter,
        [property: JsonPropertyName("max_diameter")] double MaxDiameter,
        [property: JsonPropertyName("min_eta_days")] int MinEta,
        [property: JsonPropertyName("max_eta_days")] int MaxEta,
        [property: JsonPropertyName("fillings")] ICollection<string> Fillings,
        [property: JsonPropertyName("do_images")] bool DoImages,
        [property: JsonPropertyName("do_shapes")] bool DoShapes,
        [property: JsonPropertyName("do_custom")] bool DoCustom
    );

    public record ConfectionerBalanceResponse(
        [property: JsonPropertyName("balance_available")] int BalanceAvailable,
        [property: JsonPropertyName("balance_freezed")] int BalanceFreezed
    );

    public record WithdrawRequest(
        [property: JsonPropertyName("amount")] int Amount,
        [property: JsonPropertyName("card_number")] string CardNumber,
        [property: JsonPropertyName("expiration_date")] string ExpirationDate,
        [property: JsonPropertyName("cvc")] string Cvc
    );
}
