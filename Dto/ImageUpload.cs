using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Caker.Attributes.FileAttributes;

namespace Caker.Dto
{
    public class ImageUpload
    {
        [JsonPropertyName("image")]
        [Required]
        [MaxFileSize(10 * 1024 * 1024)] // 10MB
        [AllowedExtensions([".jpg", ".jpeg", ".png"])]
        public IFormFile Image { get; set; }
    }
}
