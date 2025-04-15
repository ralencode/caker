using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class MessageRepository(CakerDbContext context) : BaseRepository<Message>(context)
    {
        protected override Expression<Func<Message, object?>>[] GetIncludes()
        {
            return [m => m.To, m => m.From];
        }

        public async Task<IEnumerable<Message>?> GetMessagesBetweenUsers(
            int firstUserId,
            int secondUserId
        )
        {
            var userRepository = new UserRepository(_context);
            var firstUser = await userRepository.GetById(firstUserId);
            var secondUser = await userRepository.GetById(secondUserId);

            if (firstUser == null && secondUser == null)
            {
                throw new InvalidOperationException(
                    $"Users {firstUserId} and {secondUserId} not found."
                );
            }
            else if (firstUser == null)
            {
                throw new InvalidOperationException($"User {firstUserId} not found");
            }
            else if (secondUser == null)
            {
                throw new InvalidOperationException($"User {secondUserId} not found");
            }

            return await base.GetWhere(m =>
                (m.FromId == firstUserId || m.FromId == secondUserId)
                && (m.ToId == firstUserId || m.ToId == secondUserId)
            );
        }
    }
}
