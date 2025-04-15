using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class FeedbackRepository(CakerDbContext context) : BaseRepository<Feedback>(context)
    {
        protected override Expression<Func<Feedback, object?>>[] GetIncludes()
        {
            return [f => f.Confectioner, f => f.Customer, f => f.Cake];
        }

        public async Task<IEnumerable<Feedback>?> GetByConfectioner(int confectionerId)
        {
            return await GetWhere(m => m.ConfectionerId == confectionerId);
        }
    }
}
