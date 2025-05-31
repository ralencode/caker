using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class OrderRepository(CakerDbContext context) : BaseRepository<Order>(context)
    {
        protected override Func<IQueryable<Order>, IQueryable<Order>> GetIncludes()
        {
            return query =>
                query
                    .Include(o => o.Cake)
                    .ThenInclude(c => c!.Confectioner)
                    .ThenInclude(conf => conf!.User)
                    .Include(o => o.Customer)
                    .ThenInclude(c => c!.User);
        }

        public async Task<IEnumerable<Order>?> GetByConfectioner(int confectionerId)
        {
            return await GetWhere(o => o.Cake!.ConfectionerId == confectionerId);
        }

        public async Task<IEnumerable<Order>?> GetByCustomer(int customerId)
        {
            return await GetWhere(o => o.CustomerId == customerId);
        }
    }
}
