namespace Tests.TestClasses;


public class TestCompanyRepository : ICompanyRepository
{
    public Task<IEnumerable<Company>> SelectAllCompaniesAsync()
        => Task.FromResult(CommonTestValues.GetCompanies().AsEnumerable());
}