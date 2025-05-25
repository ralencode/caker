using Caker.Models;

namespace Caker.Services.CurrentUserService
{
    public interface ICurrentUserService
    {
        int GetUserId();
        Task<User?> GetUser();
        bool IsAuthorized();
        UserType GetUserType();
        bool IsAdmin();
    }
}
