using System.ComponentModel.DataAnnotations;
using Caker.Models;

namespace Caker.Dto
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public UserType Type { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
    }
}
