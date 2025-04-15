using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class Confectioner : BaseModel
    {
        public required int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public required string Description { get; set; } = "";
        public required double Rating { get; set; } = 0.0;
        public required string Address { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cake>? Cakes { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}
