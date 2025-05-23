using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public enum OrderStatusType
    {
        [JsonPropertyName("pending_approval")]
        PENDING_APPROVAL,

        [JsonPropertyName("pending_payment")]
        PENDING_PAYMENT,

        [JsonPropertyName("in_progress")]
        IN_PROGRESS,

        [JsonPropertyName("done")]
        DONE,

        [JsonPropertyName("received")]
        RECEIVED,

        [JsonPropertyName("canceled")]
        CANCELED,

        [JsonPropertyName("rejected")]
        REJECTED,
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

        [JsonPropertyName("creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("order_status")]
        public OrderStatusType OrderStatus { get; set; }

        public int Quantity { get; set; }

        [JsonPropertyName("is_custom")]
        public bool IsCustom { get; set; }
    }
}
