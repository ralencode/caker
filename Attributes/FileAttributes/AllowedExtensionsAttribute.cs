using System.ComponentModel.DataAnnotations;

namespace Caker.Attributes.FileAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_extensions.Contains(extension))
                    return new ValidationResult(
                        $"Allowed extensions: {string.Join(", ", _extensions)}"
                    );
            }
            return ValidationResult.Success;
        }
    }
}
