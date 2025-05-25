using System.Security.Claims;
using Caker.Models;
using Caker.Repositories;

namespace Caker.Services.CurrentUserService
{
    public class CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        UserRepository userRepository
    ) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserRepository _userRepo = userRepository;

        public int GetUserId() =>
            int.Parse(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

        public async Task<User?> GetUser() => await _userRepo.GetById(GetUserId());

        public bool IsAuthorized() =>
            _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                is not null;

        public UserType GetUserType() =>
            Enum.Parse<UserType>(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!
            );

        public bool IsAdmin() => GetUserType() == UserType.ADMIN;
    }
}
