using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Caker.Attributes.FileAttributes;

namespace Caker.Dto
{
    public record ImageUpload
    {
        [JsonPropertyName("image")]
        [Required]
        [MaxFileSize(10 * 1024 * 1024)] // 10MB
        [AllowedExtensions([".jpg", ".jpeg", ".png"])]
        public required IFormFile? Image { get; set; }
    }
}
