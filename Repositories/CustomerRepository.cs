using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class CustomerRepository(CakerDbContext context) : BaseRepository<Customer>(context)
    {
        protected override Func<IQueryable<Customer>, IQueryable<Customer>> GetIncludes()
        {
            return query => query.Include(c => c.User);
        }
    }
}
