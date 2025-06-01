using System.Text.Json.Serialization;

namespace Caker.Models.Interfaces
{
    public interface IAccessibleBy
    {
        [JsonIgnore]
        [JsonPropertyName("allowed_user_ids")]
        ICollection<int> AllowedUserIds { get; }
    }
}
