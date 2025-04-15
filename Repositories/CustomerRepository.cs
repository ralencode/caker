using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class CustomerRepository(CakerDbContext context) : BaseRepository<Customer>(context)
    {
        protected override Expression<Func<Customer, object?>>[] GetIncludes()
        {
            return [c => c.User];
        }
    }
}
