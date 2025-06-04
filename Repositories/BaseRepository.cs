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

        protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query)
        {
            return GetIncludes()(query);
        }

        protected virtual Func<IQueryable<T>, IQueryable<T>> GetIncludes()
        {
            return query => query;
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

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await ApplyIncludes(_dbSet.AsNoTracking()).ToListAsync();
        }

        public virtual async Task<T?> GetById(int? id)
        {
            return await GetBy(e => e.Id == id);
        }

        protected virtual async Task<T?> GetBy(Expression<Func<T, bool>> predicate)
        {
            return await ApplyIncludes(_dbSet.AsNoTracking()).FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T?> GetById(int? id, bool tracking = false)
        {
            return await GetBy(e => e.Id == id, tracking);
        }

        protected virtual async Task<T?> GetBy(
            Expression<Func<T, bool>> predicate,
            bool tracking = false
        )
        {
            var query = ApplyIncludes(_dbSet);
            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(predicate);
        }

        protected virtual async Task<IEnumerable<T>> GetWhere(
            params Expression<Func<T, bool>>[] predicates
        )
        {
            var query = ApplyIncludes(_dbSet.AsNoTracking());
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
            return await query.Distinct().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetWhereOrdered<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> keySelector,
            bool ascending = true,
            bool handleNulls = false
        )
        {
            var query = ApplyIncludes(_dbSet.AsNoTracking()).Where(predicate);

            if (handleNulls)
            {
                // Create null check expression
                var parameter = keySelector.Parameters[0];
                var nullCheck = Expression.Equal(
                    keySelector.Body,
                    Expression.Constant(null, typeof(TKey))
                );
                var lambda = Expression.Lambda<Func<T, bool>>(nullCheck, parameter);

                if (ascending)
                {
                    query = query.OrderBy(lambda).ThenBy(keySelector);
                }
                else
                {
                    query = query.OrderByDescending(lambda).ThenByDescending(keySelector);
                }
            }
            else
            {
                query = ascending
                    ? query.OrderBy(keySelector)
                    : query.OrderByDescending(keySelector);
            }

            return await query.ToListAsync();
        }

        public async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            var existing = _context
                .ChangeTracker.Entries<T>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id)
                ?.Entity;

            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbSet.Update(entity);
            }
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
