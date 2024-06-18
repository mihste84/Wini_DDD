using System.Transactions;

namespace DatabaseCommon;

public class TransactionScopeWrapper : ITransactionScope
{
    private bool _disposed;
    private readonly TransactionScope _scope;

    public TransactionScopeWrapper()
    {
        _scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    public void Complete()
    {
        _scope.Complete();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _scope.Dispose();
        }

        _disposed = true;
    }
}