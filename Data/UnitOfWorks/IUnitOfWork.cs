using DataContextLib.Models;
using DataContextLib.Repository;
using Microsoft.EntityFrameworkCore;

namespace DataContextLib.UnitOfWorks;

public interface IUnitOfWork<out TContext> where TContext : DbContext
{
    TContext Context { get; }
    Task CreateTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveAsync();
    IRepository<T> GetRepository<T>() where T : BaseEntity;
}