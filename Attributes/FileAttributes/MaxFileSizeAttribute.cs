using System.ComponentModel.DataAnnotations;

namespace Caker.Attributes.FileAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MaxFileSizeAttribute(int maxFileSize) : ValidationAttribute
    {
        private readonly int _maxFileSize = maxFileSize;

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is null)
                return ValidationResult.Success;
            if (value is IFormFile file && file.Length > _maxFileSize)
                return new ValidationResult($"Maximum file size is {_maxFileSize / 1024 / 1024}MB");

            return ValidationResult.Success;
        }
    }
}
