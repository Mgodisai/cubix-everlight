using Data.Specifications;

namespace Data.Repository
{
    public interface IRepository<T> where T : class
    {
        T? GetById(Guid id);
        IEnumerable<T> FindWithSpecification(ISpecification<T> spec);
        IEnumerable<T> GetAll();
        T Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
