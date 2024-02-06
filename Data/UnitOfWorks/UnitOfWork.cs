using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Entity.Validation;
using DataContextLib.Models;
using DataContextLib.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataContextLib.UnitOfWorks;
public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
{
    public TContext Context { get; }
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction _transaction;
    private bool _disposed;
    private string _errorMessage = string.Empty;

    public UnitOfWork(TContext context)
    {
        Context = context;
        _repositories = [];
    }

    public async Task CreateTransactionAsync()
    {
        _transaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
        _transaction.Dispose();
    }

    public async Task SaveAsync()
    {
        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbEntityValidationException dbEx)
        {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    _errorMessage += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage} {Environment.NewLine}";
                }
            }
            throw new Exception(_errorMessage, dbEx);
        }
    }

    public IRepository<T> GetRepository<T>() where T : BaseEntity
    {
        if (!_repositories.ContainsKey(typeof(T)))
        {
            var repositoryType = typeof(DataRepository<>).MakeGenericType(typeof(T));
            var repositoryLogger = Context.GetService<ILoggerFactory>().CreateLogger(repositoryType);
            var repositoryInstance = (IRepository<T>)Activator.CreateInstance(
                repositoryType,
                new object[] { Context, repositoryLogger }
            );
            _repositories.Add(typeof(T), repositoryInstance);
            var repository = new DataRepository<T>(Context, repositoryLogger);
        }
        return (IRepository<T>)_repositories[typeof(T)];
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                Context.Dispose();
        _disposed = true;
    }
}