using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public enum PaymentStatusType
    {
        Pending,
        Paid,
        Refunded,
    }

    public enum OrderStatusType
    {
        Pending,
        InProgress,
        Delivered,
        Canceled,
    }

    public class Order : BaseModel
    {
        [JsonPropertyName("cake_id")]
        public required int CakeId { get; set; }

        [ForeignKey("CakeId")]
        public virtual Cake? Cake { get; set; }

        [JsonPropertyName("customer_id")]
        public required int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public required double Price { get; set; }

        [JsonPropertyName("payment_status")]
        public PaymentStatusType PaymentStatus { get; set; }

        [JsonPropertyName("creation_date")]
        public required DateTime CreationDate { get; set; }
        public required TimeSpan Eta { get; set; }

        [JsonPropertyName("order_status")]
        public OrderStatusType OrderStatus { get; set; }
    }
}
