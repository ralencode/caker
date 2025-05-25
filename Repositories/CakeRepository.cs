using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public class CakeRepository(CakerDbContext context) : BaseRepository<Cake>(context)
    {
        protected override Expression<Func<Cake, object?>>[] GetIncludes()
        {
            return [c => c.Confectioner, c => c.Orders];
        }

        public override async Task<Cake?> GetById(int? id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Confectioner)
                .ThenInclude(conf => conf!.User) // Ensure User is included
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cake>?> GetByConfectioner(int confectionerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ConfectionerId == confectionerId)
                .Include(c => c.Confectioner)
                .ThenInclude(conf => conf!.User)
                .Include(c => c.Orders)
                .ToListAsync();
        }
    }
}
