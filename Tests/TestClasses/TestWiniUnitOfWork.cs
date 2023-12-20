namespace Tests.TestClasses;


public class TestWiniUnitOfWork : IWiniUnitOfWork
{
    public ICompanyRepository CompanyRepository => new TestCompanyRepository();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}