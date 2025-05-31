using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class CakeRepository(CakerDbContext context) : BaseRepository<Cake>(context)
    {
        protected override Func<IQueryable<Cake>, IQueryable<Cake>> GetIncludes()
        {
            return query =>
                query
                    .Include(c => c.Confectioner)
                    .ThenInclude(conf => conf!.User)
                    .Include(c => c.Orders);
        }

        public async Task<IEnumerable<Cake>?> GetByConfectioner(int confectionerId)
        {
            return await GetWhere(c => c.ConfectionerId == confectionerId);
        }
    }
}
