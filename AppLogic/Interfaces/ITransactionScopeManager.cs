namespace AppLogic.Interfaces;

public interface ITransactionScopeManager
{
    public ITransactionScope CreateTransaction();
}