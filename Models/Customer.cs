using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Caker.Dto;

namespace Caker.Models
{
    public class Customer : BaseModel, IDtoable<CustomerResponse>
    {
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; }

        public CustomerResponse ToDto() => new(Id, User!.Name, User.PhoneNumber, User.Email);
    }
}
