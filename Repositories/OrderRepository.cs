using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class OrderRepository(CakerDbContext context) : BaseRepository<Order>(context)
    {
        protected override Expression<Func<Order, object?>>[] GetIncludes()
        {
            return [o => o.Cake!, o => o.Customer!];
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
