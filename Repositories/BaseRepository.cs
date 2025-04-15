using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;
using Microsoft.EntityFrameworkCore;

namespace Caker.Repositories
{
    public abstract class BaseRepository<T>
        where T : BaseModel
    {
        protected readonly CakerDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(CakerDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        protected virtual IQueryable<T> GetQuery(params Expression<Func<T, object?>>[] includes)
        {
            var query = _dbSet.AsNoTracking();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        protected virtual Expression<Func<T, object?>>[] GetIncludes()
        {
            return [];
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await GetQuery(GetIncludes() ?? []).ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await GetBy(e => e.Id == id);
        }

        protected virtual async Task<T?> GetBy(
            Expression<Func<T, bool>> predicate
        )
        {
            return await GetQuery(GetIncludes() ?? [])
                .FirstOrDefaultAsync(predicate)
                .ConfigureAwait(false);
        }

        protected virtual async Task<IEnumerable<T>> GetWhere(
            params Expression<Func<T, bool>>[] predicates
        )
        {
            var query = GetQuery(GetIncludes() ?? []);
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
            return await query.Distinct().ToListAsync().ConfigureAwait(false);
        }

        public async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
