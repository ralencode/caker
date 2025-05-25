using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class CakeRepository(CakerDbContext context) : BaseRepository<Cake>(context)
    {
        protected override Expression<Func<Cake, object?>>[] GetIncludes()
        {
            return [c => c.Confectioner, c => c.Orders];
        }

        public async Task<IEnumerable<Cake>?> GetByConfectioner(int confectionerId)
        {
            return await GetWhere(c => c.ConfectionerId == confectionerId);
        }
    }
}
