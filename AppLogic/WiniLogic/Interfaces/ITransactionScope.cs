namespace AppLogic.WiniLogic.Interfaces;

public interface ITransactionScope : IDisposable
{
    public void Complete();
}