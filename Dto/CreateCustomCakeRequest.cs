using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public class CreateCustomCakeRequest : ImageUpload
    {
        [JsonPropertyName("confectioner_id")]
        public int ConfectionerId { get; set; }

        [JsonPropertyName("fillings")]
        public List<string> Fillings { get; set; }

        [JsonPropertyName("eta_days")]
        public int EtaDays { get; set; }

        [JsonPropertyName("color")]
        public long Color { get; set; }

        [JsonPropertyName("diameter")]
        public double Diameter { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("text_x")]
        public double TextX { get; set; }

        [JsonPropertyName("text_y")]
        public double TextY { get; set; }

        [JsonPropertyName("text_size")]
        public double TextSize { get; set; }
    }
}
