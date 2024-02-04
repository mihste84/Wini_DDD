namespace AppLogic.Interfaces;

public interface IUnitOfWork
{
    ICompanyRepository CompanyRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
