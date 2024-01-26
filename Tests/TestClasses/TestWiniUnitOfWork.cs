namespace Tests.TestClasses;

public class TestWiniUnitOfWork : IWiniUnitOfWork
{
    public ICompanyRepository CompanyRepository { get; }

    public TestWiniUnitOfWork(ICompanyRepository repo)
    {
        CompanyRepository = repo;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}