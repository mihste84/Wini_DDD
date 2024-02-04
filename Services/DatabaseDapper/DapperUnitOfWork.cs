namespace Services.DatabaseDapper;

public class DapperUnitOfWork : IUnitOfWork, IDisposable
{
    private bool _disposed;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private readonly ILogger<DapperUnitOfWork> _logger;

    private ICompanyRepository? _companyRepository;
    public ICompanyRepository CompanyRepository => _companyRepository ??= new CompanyRepository(_transaction);

    public DapperUnitOfWork(IDbConnection connection, ILogger<DapperUnitOfWork> logger)
    {
        _connection = connection;
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _logger = logger;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
            throw new NullReferenceException("Database connection is not created.");

        if (_transaction == null)
            throw new NullReferenceException("Database transaction is not created.");

        try
        {
            _transaction.Commit();
        }
        catch (SqlException ex)
        {
            LogAllSqlExceptions(ex);
            _transaction.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred while saving changes");
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
            ClearRepositories();
        }

        return Task.CompletedTask;
    }

    private void LogAllSqlExceptions(Exception ex)
    {
        _logger?.LogError(ex, "A database error occurred while saving changes.");
        if (ex.InnerException != null)
            LogAllSqlExceptions(ex.InnerException);
    }

    private void ClearRepositories()
    {
        _companyRepository = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection?.Dispose();
                _connection = null;
            }
        }

        _disposed = true;
    }
}