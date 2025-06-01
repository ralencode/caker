using System.Text.Json.Serialization;
using Caker.Dto;
using Caker.Models.Interfaces;

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

    public class User : BaseModel, IDtoable<UserResponse>, IAccessibleBy
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        [JsonIgnore]
        public required string Password { get; set; }

        [JsonPropertyName("phone_number")]
        public required string PhoneNumber { get; set; }

        public string? Address { get; set; } = "";

        public string? Description { get; set; } = "";

        public UserType Type { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Confectioner? Confectioner { get; set; }

        [JsonIgnore]
        [JsonPropertyName("refresh_tokens")]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public ICollection<int> AllowedUserIds => [Id];

        public UserResponse ToDto() =>
            new(
                Id,
                Name,
                PhoneNumber,
                Email,
                Address,
                Description,
                Type,
                Customer?.ToDto(),
                Confectioner?.ToDto()
            );
    }
}
