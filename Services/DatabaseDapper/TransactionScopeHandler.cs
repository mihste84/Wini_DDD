using System.Transactions;

namespace Services.DatabaseDapper;

public class TransactionScopeHandler : ITransactionHandler
{
    private bool _disposed;
    private TransactionScope? Transaction { get; set; }
    public async Task CompleteAsync()
    {
        if (Transaction == default)
            throw new InvalidOperationException("Transaction not started.");

        try
        {
            Transaction?.Complete();
        }
        finally
        {
            Transaction?.Dispose();
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync()
    {
        if (Transaction != default)
            throw new InvalidOperationException("Transaction is already active.");

        Transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Transaction?.Dispose();
            Transaction = null!;
        }

        _disposed = true;
    }
}