using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Caker.Dto;
using Caker.Models.Interfaces;

namespace Caker.Models
{
    public class Confectioner : BaseModel, IDtoable<ConfectionerResponse>, IAccessibleBy
    {
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public double Rating { get; set; } = 0.0;

        [JsonPropertyName("max_diameter")]
        public double MaxDiameter { get; set; } = 100.0;

        [JsonPropertyName("min_diameter")]
        public double MinDiameter { get; set; } = 0.0;

        [JsonPropertyName("max_eta")]
        public int MaxEta { get; set; } = 2048;

        [JsonPropertyName("min_eta")]
        public int MinEta { get; set; } = 0;

        public ICollection<string>? Fillings { get; set; }

        [JsonPropertyName("do_images")]
        public bool DoImages { get; set; }

        [JsonPropertyName("do_shapes")]
        public bool DoShapes { get; set; }

        [JsonPropertyName("do_custom")]
        public bool DoCustom { get; set; }

        [JsonPropertyName("balance_available")]
        public int BalanceAvailable { get; set; }

        [JsonPropertyName("balance_freezed")]
        public int BalanceFreezed { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cake>? Cakes { get; set; }

        public ICollection<int> AllowedUserIds => [UserId];

        public ConfectionerResponse ToDto() =>
            new(
                Id,
                User!.Name,
                User.PhoneNumber,
                User.Email,
                User.Description ?? "",
                Rating,
                User.Address ?? "",
                MinDiameter,
                MaxDiameter,
                MinEta,
                MaxEta,
                Fillings ?? [],
                DoImages,
                DoShapes,
                DoCustom
            );
    }
}
