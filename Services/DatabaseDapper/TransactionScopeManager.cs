namespace Services.DatabaseDapper;

public class TransactionScopeManager : ITransactionScopeManager
{
    public ITransactionScope CreateTransaction()
        => new TransactionScopeWrapper();
}