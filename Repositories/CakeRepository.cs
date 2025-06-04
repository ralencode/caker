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
                    .Include(c => c.Orders)
                    .ThenInclude(o => o.Customer)
                    .ThenInclude(c => c!.User);
        }

        public async Task<IEnumerable<Cake>> GetSortedNonCustomByWeight(bool ascending)
        {
            return await GetWhereOrdered(c => c.IsCustom == false, c => c.Weight, ascending, true);
        }

        public async Task<IEnumerable<Cake>> GetSortedNonCustomByPrice(bool ascending)
        {
            return await GetWhereOrdered(c => c.IsCustom == false, c => c.Price, ascending);
        }

        public async Task<IEnumerable<Cake>> SearchSortedNonCustomByWeight(
            bool ascending,
            string name
        )
        {
            return await GetWhereOrdered(
                c => c.Name.Contains(name) && !c.IsCustom,
                c => c.Weight,
                ascending,
                true
            );
        }

        public async Task<IEnumerable<Cake>> SearchSortedNonCustomByPrice(
            bool ascending,
            string name
        )
        {
            return await GetWhereOrdered(
                c => c.Name.Contains(name) && !c.IsCustom,
                c => c.Price,
                ascending
            );
        }

        public async Task<IEnumerable<Cake>> SearchByName(string name)
        {
            return await GetWhereOrdered(
                c => c.Name.Contains(name) && !c.IsCustom,
                c => c.Price,
                false
            );
        }

        public async Task<IEnumerable<Cake>?> GetByConfectioner(int confectionerId)
        {
            return await GetWhere(c => c.ConfectionerId == confectionerId);
        }
    }
}
