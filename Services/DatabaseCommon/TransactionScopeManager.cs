namespace DatabaseCommon;

public class TransactionScopeManager : ITransactionScopeManager
{
    public ITransactionScope CreateTransaction()
    => new TransactionScopeWrapper();
}