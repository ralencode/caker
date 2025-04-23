using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class User : BaseModel
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        [JsonPropertyName("phone_number")]
        public required string PhoneNumber { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Confectioner? Confectioner { get; set; }

        [JsonIgnore]
        [JsonPropertyName("messages_from")]
        public virtual ICollection<Message>? MessagesFrom { get; set; }

        [JsonIgnore]
        [JsonPropertyName("messages_to")]
        public virtual ICollection<Message>? MessagesTo { get; set; }
    }
}
