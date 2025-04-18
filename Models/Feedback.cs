using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class Feedback : BaseModel
    {
        [JsonPropertyName("confectioner_id")]
        public required int ConfectionerId { get; set; }

        [ForeignKey("ConfectionerId")]
        public virtual Confectioner? Confectioner { get; set; }
        public required int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
        public required int CakeId { get; set; }

        [ForeignKey("CakeId")]
        public virtual Cake? Cake { get; set; }
        public string? Image { get; set; }
        public string? Text { get; set; }
        public required int Rating { get; set; }
        public required DateTime Date { get; set; }
    }
}
