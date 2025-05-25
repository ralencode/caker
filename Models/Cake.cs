using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Caker.Dto;

namespace Caker.Models
{
    public class Cake : BaseModel, IDtoable<CakeResponse>
    {
        [JsonPropertyName("confectioner_id")]
        public required int ConfectionerId { get; set; }

        [ForeignKey("ConfectionerId")]
        public virtual Confectioner? Confectioner { get; set; }
        public required bool Visible { get; set; } = true;
        public required string Name { get; set; }
        public required string Description { get; set; } = "";
        public string Image { get; set; } = "";
        public required int Price { get; set; } = 0;
        public double? Weight { get; set; }
        public double? Diameter { get; set; }

        [JsonPropertyName("required_time")]
        public int? ReqTime { get; set; }
        public string? Text { get; set; }
        public string? Color { get; set; }
        public string? Taste { get; set; }

        [JsonPropertyName("is_custom")]
        public bool IsCustom { get; set; }
        public ICollection<string> Fillings { get; set; } = [];

        [JsonPropertyName("text_size")]
        public double TextSize { get; set; }

        [JsonPropertyName("text_y")]
        public double TextY { get; set; }

        [JsonPropertyName("text_x")]
        public double TextX { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; }

        public CakeResponse ToDto() =>
            new(
                Id,
                Confectioner?.ToDto(),
                Name,
                Description,
                Fillings,
                ReqTime,
                Color,
                $"assets/{ImagePath}",
                Price,
                Diameter,
                Weight,
                Text,
                TextSize,
                TextX,
                TextY,
                IsCustom
            );
    }
}
