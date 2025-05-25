using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Caker.Dto;
using Caker.Models.Interfaces;

namespace Caker.Models
{
    public class Customer : BaseModel, IDtoable<CustomerResponse>, IAccessibleBy
    {
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; }

        public ICollection<int> AllowedUserIds => [UserId];

        public CustomerResponse ToDto() => new(Id, User!.Name, User.PhoneNumber, User.Email);
    }
}
