using System.Text.Json.Serialization;
using Caker.Models;

namespace Caker.Dto
{
    public record CreateOrderRequest(
        [property: JsonPropertyName("cake_id")] int CakeId,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("quantity")] int Quantity
    );

    public record CreateOrderFullRequest(
        [property: JsonPropertyName("customer_id")] int CustomerId,
        [property: JsonPropertyName("cake_id")] int CakeId,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("order_status")] OrderStatusType OrderStatus,
        [property: JsonPropertyName("is_custom")] bool IsCustom,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt
    );

    public record UpdateOrderRequest(
        [property: JsonPropertyName("customer_id")] int? CustomerId,
        [property: JsonPropertyName("cake_id")] int? CakeId,
        [property: JsonPropertyName("price")] double? Price,
        [property: JsonPropertyName("quantity")] int? Quantity
    );

    public record UpdateOrderFullRequest(
        [property: JsonPropertyName("customer_id")] int? CustomerId,
        [property: JsonPropertyName("cake_id")] int? CakeId,
        [property: JsonPropertyName("price")] double? Price,
        [property: JsonPropertyName("quantity")] int? Quantity,
        [property: JsonPropertyName("order_status")] OrderStatusType? OrderStatus,
        [property: JsonPropertyName("is_custom")] bool? IsCustom,
        [property: JsonPropertyName("created_at")] DateTime? CreatedAt
    );

    public record OrderResponse(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("customer")] CustomerResponse Customer,
        [property: JsonPropertyName("confectioner")] ConfectionerResponse Confectioner,
        [property: JsonPropertyName("cake")] CakeResponse Cake,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("status")] OrderStatusType Status,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("is_custom")] bool IsCustom
    );

    public record PaymentRequest(
        [property: JsonPropertyName("card_number")] string CardNumber,
        [property: JsonPropertyName("expiration_date")] string ExpirationDate,
        [property: JsonPropertyName("cvc")] string Cvc
    );

    public record BatchPaymentRequest(
        [property: JsonPropertyName("orders")] ICollection<CreateOrderRequest> Orders,
        [property: JsonPropertyName("card_number")] string CardNumber,
        [property: JsonPropertyName("expiration_date")] string ExpirationDate,
        [property: JsonPropertyName("cvc")] string Cvc
    );
}
