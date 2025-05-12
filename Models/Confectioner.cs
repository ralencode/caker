using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class Confectioner : BaseModel
    {
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public required string Description { get; set; } = "";
        public double Rating { get; set; } = 0.0;
        public required string Address { get; set; }

        [JsonPropertyName("max_diameter")]
        public required double MaxDiameter { get; set; } = double.PositiveInfinity;

        [JsonPropertyName("min_diameter")]
        public required double MinDiameter { get; set; } = 0.0;

        [JsonPropertyName("max_eta")]
        public int MaxEta { get; set; } = 2048;

        [JsonPropertyName("min_eta")]
        public int MinEta { get; set; } = 0;

        public ICollection<string>? Tastes { get; set; }

        [JsonPropertyName("do_images")]
        public bool DoImages { get; set; }

        [JsonPropertyName("do_shapes")]
        public bool DoShapes { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cake>? Cakes { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}
