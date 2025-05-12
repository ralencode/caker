using System.Text.Json.Serialization;

namespace Caker.Dto
{
    public class CreateRegularCakeRequest : ImageUpload
    {
        [JsonPropertyName("confectioner_id")]
        public int ConfectionerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("diameter")]
        public double Diameter { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        [JsonPropertyName("eta_days")]
        public int EtaDays { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }
    }
}
