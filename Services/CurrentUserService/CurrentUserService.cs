using System.Security.Claims;
using Caker.Models;

namespace Caker.Services.CurrentUserService
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public int GetUserId() =>
            int.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

        public UserType GetUserType() =>
            Enum.Parse<UserType>(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!
            );

        public bool IsAdmin() => GetUserType() == UserType.ADMIN;
    }
}
