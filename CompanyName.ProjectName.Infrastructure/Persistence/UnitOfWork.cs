using CompanyName.ProjectName.Application.Common.Interfaces;

namespace CompanyName.ProjectName.Infrastructure.Persistence;

internal sealed class UnitOfWork(IDbConnection connection) : IUnitOfWork
{
    private IDbTransaction? _transaction;
    private readonly Dictionary<string, object> _repositories = [];

    private void EnsureOpenConnection()
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var key = typeof(T).Name;

        if (!_repositories.ContainsKey(key))
        {
            EnsureOpenConnection();
            _repositories[key] = new GenericRepository<T>(connection, _transaction);
        }

        return (IGenericRepository<T>)_repositories[key];
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            if (_transaction != null)
            {
                _transaction.Commit();
            }

            return await Task.FromResult(1);
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
            _repositories.Clear();
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        connection.Dispose();
    }
}