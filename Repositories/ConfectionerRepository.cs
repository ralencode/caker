using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class ConfectionerRepository(CakerDbContext context)
        : BaseRepository<Confectioner>(context)
    {
        protected override Func<IQueryable<Confectioner>, IQueryable<Confectioner>> GetIncludes()
        {
            return query => query.Include(c => c.User).Include(c => c.Cakes);
        }
    }
}
