using System.ComponentModel.DataAnnotations.Schema;

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
        public required int CakeId { get; set; }

        [ForeignKey("CakeId")]
        public virtual Cake? Cake { get; set; }
        public required int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public required double Price { get; set; }
        public PaymentStatusType PaymentStatus { get; set; }
        public required DateTime CreationDate { get; set; }
        public required TimeSpan Eta { get; set; }
        public OrderStatusType OrderStatus { get; set; }
    }
}
