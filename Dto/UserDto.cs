using System.Text.Json.Serialization;
using Caker.Models;

namespace Caker.Dto
{
    public record CreateUserRequest(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("password")] string Password,
        [property: JsonPropertyName("type")] UserType Type
    );

    public record UpdateUserRequest(
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("phone")] string? Phone,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("type")] UserType? Type
    );

    public record UserResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("phone")] string Phone,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("type")] UserType Type,
        [property: JsonPropertyName("customer")] CustomerResponse? Customer,
        [property: JsonPropertyName("confectioner")] ConfectionerResponse? Confectioner
    );
}
