namespace AppLogic.Interfaces;

public interface ITransactionScope : IDisposable
{
    public void Complete();
}