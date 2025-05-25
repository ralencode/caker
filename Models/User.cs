using System.Text.Json.Serialization;
using Caker.Dto;

namespace Caker.Models
{
    public enum UserType
    {
        [JsonPropertyName("confectioner")]
        CONFECTIONER,

        [JsonPropertyName("customer")]
        CUSTOMER,

        [JsonPropertyName("admin")]
        ADMIN,
    }

    public class User : BaseModel, IDtoable<UserResponse>
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        [JsonPropertyName("phone_number")]
        public required string PhoneNumber { get; set; }

        public UserType Type { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Confectioner? Confectioner { get; set; }

        [JsonIgnore]
        [JsonPropertyName("refresh_tokens")]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public UserResponse ToDto() =>
            new(Id, Name, PhoneNumber, Email, Type, Customer?.ToDto(), Confectioner?.ToDto());
    }
}
