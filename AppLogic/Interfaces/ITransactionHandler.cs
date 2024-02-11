namespace AppLogic.Interfaces;

public interface ITransactionHandler : IDisposable
{
    Task StartAsync();
    Task CompleteAsync();
}