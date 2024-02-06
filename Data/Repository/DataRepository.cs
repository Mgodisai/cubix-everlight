using DataContextLib.Models;
using DataContextLib.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataContextLib.Repository
{
    public class DataRepository<T> : IRepository<T>, IDisposable where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger _logger;
        private bool _isDisposed;

        public DataRepository(DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            var query = _dbSet.AsQueryable();

            if (spec.Criteria != null)
            {
                foreach (var criteria in spec.Criteria)
                {
                    query = query.Where(criteria);
                }
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting entity with id {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<T>> FindWithSpecificationAsync(ISpecification<T> spec)
        {
            var query = ApplySpecification(spec);
            return await query.ToListAsync();
        }

        public async Task<bool> InsertAsync(T entity)
        {
            try
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = entity.CreatedAt;
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error inserting entity: {entity}", entity);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Entity with id {Id} not found for deletion", id);
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting entity with id {Id}", id);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                entity.UpdatedAt = DateTime.UtcNow;
                await Task.Delay(10);
                _context.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating entity: {entity}", entity);
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
                if (disposing)
                    _context.Dispose();
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
