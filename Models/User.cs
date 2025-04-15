using System.Text.Json.Serialization;

namespace Caker.Models
{
    public class User : BaseModel
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string PhoneNumber { get; set; }

        [JsonIgnore]
        public virtual Customer? Customer { get; set; }

        [JsonIgnore]
        public virtual Confectioner? Confectioner { get; set; }

        [JsonIgnore]
        public virtual ICollection<Message>? MessagesFrom { get; set; }

        [JsonIgnore]
        public virtual ICollection<Message>? MessagesTo { get; set; }
    }
}
