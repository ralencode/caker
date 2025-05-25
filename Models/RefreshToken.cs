using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Caker.Dto;

namespace Caker.Models
{
    public class RefreshToken : BaseModel, IDtoable<RefreshTokenResponse>
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public string Token { get; set; } = null!;

        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }

        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [JsonIgnore]
        public bool IsActive => Revoked == null && !IsExpired;

        public RefreshTokenResponse ToDto() =>
            new(User!.ToDto(), Token, Expires, Revoked, IsExpired, IsActive);
    }
}
