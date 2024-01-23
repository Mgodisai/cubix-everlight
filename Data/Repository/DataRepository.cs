using Data.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository
{
    public class DataRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        private IDbContextTransaction? _currentTransaction;

        public DataRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public T Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbSet.Add(entity);

            ExecuteInTransaction(() => _context.SaveChanges());

            return entity;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);

            ExecuteInTransaction(() => _context.SaveChanges());
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<T> FindWithSpecification(ISpecification<T> spec)
        {
            var query = ApplySpecification(spec);
            return query.AsNoTracking();
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

        public T? GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            ExecuteInTransaction(() => _context.SaveChanges());
        }

        private void ExecuteInTransaction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                if (_currentTransaction == null)
                {
                    BeginTransaction();
                }

                action();

                _context.SaveChanges();

                if (_currentTransaction != null)
                {
                    Commit();
                }
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        public void BeginTransaction()
        {
            _currentTransaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _currentTransaction?.Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Rollback()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
