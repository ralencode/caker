using System.ComponentModel.DataAnnotations;

namespace Caker.Dto
{
    public class LoginRequest
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
