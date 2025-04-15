using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class UserRepository(CakerDbContext context) : BaseRepository<User>(context)
    {
        protected override Expression<Func<User, object?>>[] GetIncludes()
        {
            return [u => u.MessagesTo, u => u.MessagesFrom, u => u.Customer, u => u.Confectioner];
        }

        public async Task<User?> GetByPhoneNumber(string phoneNumber)
        {
            return await base.GetBy(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<IEnumerable<User>?> GetUsersWithChat(int userId)
        {
            var user =
                await base.GetById(userId)
                ?? throw new InvalidOperationException($"User with id {userId} not found.");

            return (await base.GetWhere(u => u.Id != userId)).Where(u =>
                (u.MessagesFrom?.Any(m => m.ToId == userId) ?? false)
                || (u.MessagesTo?.Any(m => m.FromId == userId) ?? false)
            );
        }
    }
}
