using System.Linq.Expressions;

namespace DataContextLib.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public List<Expression<Func<T, bool>>> Criteria { get; } = [];
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; } = false;

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddCriteria(Expression<Func<T, bool>> criteriaExpression)
    {

        Criteria.Add(criteriaExpression);
    }

    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}