using Caker.Models;

namespace Caker.Services.CurrentUserService
{
    public interface ICurrentUserService
    {
        int GetUserId();
        UserType GetUserType();
        bool IsAdmin();
    }
}
