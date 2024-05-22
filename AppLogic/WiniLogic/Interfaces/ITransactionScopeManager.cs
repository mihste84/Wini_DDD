namespace AppLogic.WiniLogic.Interfaces;

public interface ITransactionScopeManager
{
    public ITransactionScope CreateTransaction();
}