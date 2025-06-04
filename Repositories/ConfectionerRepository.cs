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

        public async Task<IEnumerable<Confectioner>> GetSortedByCakesCount(bool ascending)
        {
            return await GetWhereOrdered(
                c => c.Cakes.Where(c => !c.IsCustom).Any() || c.DoCustom,
                c => c.Cakes.Where(c => !c.IsCustom).Count(),
                ascending
            );
        }

        public async Task<IEnumerable<Confectioner>> SearchByName(string name)
        {
            return await GetWhereOrdered(
                c => c.User!.Name.Contains(name),
                c => c.Cakes.Where(c => !c.IsCustom).Count(),
                false
            );
        }
    }
}
