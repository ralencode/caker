using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class Customer : BaseModel
    {
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}
