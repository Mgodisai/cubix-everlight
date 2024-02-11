using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Entity.Validation;
using DataContextLib.Models;
using DataContextLib.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataContextLib.UnitOfWorks;
public class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
{
    public TContext Context { get; } = context;
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction? _transaction;
    public IDbContextTransaction? CurrentTransaction
    {
        get
        {
            return _transaction;
        }
    }

    private bool _disposed;
    private string _errorMessage = string.Empty;

    public async Task CreateTransactionAsync()
    {
        _transaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No active transaction");
        }
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No active transaction");
        }
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
            var repositoryType = typeof(DataRepository<>).MakeGenericType(typeof(T)) ?? throw new InvalidOperationException("Repository type cannot be null.");
            var repositoryLogger = Context.GetService<ILoggerFactory>().CreateLogger(repositoryType) ?? throw new InvalidOperationException("Repository logger cannot be null.");
            var ob = new object[] { Context, repositoryLogger };
            var repositoryInstance = (IRepository<T>?)Activator.CreateInstance(repositoryType, ob)
                ?? throw new InvalidOperationException($"Unable to create an instance of {repositoryType}.");
            _repositories.Add(typeof(T), repositoryInstance);
        }
        return (IRepository<T>)_repositories[typeof(T)] ?? throw new InvalidOperationException("A repository instance cannot be null.");
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