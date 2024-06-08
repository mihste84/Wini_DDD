namespace Services.DatabaseCommon;

public class TransactionScopeManager : ITransactionScopeManager
{
    public ITransactionScope CreateTransaction()
    => new TransactionScopeWrapper();
}