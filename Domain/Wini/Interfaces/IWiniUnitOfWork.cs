namespace Domain.Wini.Interfaces;

public interface IWiniUnitOfWork
{
    ICompanyRepository CompanyRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
