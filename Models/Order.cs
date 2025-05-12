using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public enum OrderStatusType
    {
        PENDING_APPROVAL,
        PENDING_PAYMENT,
        IN_PROGRESS,
        DONE,
        RECEIVED,
        CANCELED,
        REJECTED,
    }

    public enum PaymentStatusType
    {
        PENDING,
        PAID,
        REFUNDED,
    }

    public class Order : BaseModel
    {
        [JsonPropertyName("cake_id")]
        public int CakeId { get; set; }

        [ForeignKey("CakeId")]
        public virtual Cake? Cake { get; set; }

        [JsonPropertyName("customer_id")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public double Price { get; set; }

        [JsonPropertyName("payment_status")]
        public PaymentStatusType PaymentStatus { get; set; }

        [JsonPropertyName("creation_date")]
        public DateTime CreationDate { get; set; }
        public TimeSpan Eta { get; set; }

        [JsonPropertyName("order_status")]
        public OrderStatusType OrderStatus { get; set; }
    }
}
