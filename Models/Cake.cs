using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class Cake : BaseModel
    {
        [JsonPropertyName("confectioner_id")]
        public required int ConfectionerId { get; set; }

        [ForeignKey("ConfectionerId")]
        public virtual Confectioner? Confectioner { get; set; }
        public required bool Visible { get; set; } = true;
        public required string Name { get; set; }
        public required string Description { get; set; } = "";
        public required string Image { get; set; } = "";
        public required int Price { get; set; } = 0;
        public double? Weight { get; set; }
        public double? Diameter { get; set; }

        [JsonPropertyName("req_time")]
        public TimeSpan? ReqTime { get; set; }
        public string? Text { get; set; }
        public string? Color { get; set; }
        public string? Taste { get; set; }

        [JsonPropertyName("is_custom")]
        public bool IsCustom { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}
