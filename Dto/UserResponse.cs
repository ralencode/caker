using Caker.Models;

namespace Caker.Dto
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
    }
}
