using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class UserRepository(CakerDbContext context) : BaseRepository<User>(context)
    {
        protected override Func<IQueryable<User>, IQueryable<User>> GetIncludes()
        {
            return query =>
                query
                    .Include(u => u.RefreshTokens)
                    .Include(u => u.Customer)
                    .Include(u => u.Confectioner);
        }

        public async Task<User?> GetByPhoneNumber(string phoneNumber)
        {
            return await base.GetBy(u => u.PhoneNumber == phoneNumber);
        }
    }
}
