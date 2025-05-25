using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class UserRepository(CakerDbContext context) : BaseRepository<User>(context)
    {
        protected override Expression<Func<User, object?>>[] GetIncludes()
        {
            return [u => u.RefreshTokens, u => u.Customer, u => u.Confectioner];
        }

        public async Task<User?> GetByPhoneNumber(string phoneNumber)
        {
            return await base.GetBy(u => u.PhoneNumber == phoneNumber);
        }
    }
}
