namespace Services.DatabaseDapper.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly IDbTransaction? _transaction;

    public CompanyRepository(IDbTransaction? transaction)
    {
        _transaction = transaction;
    }

    public async Task<IEnumerable<Company>> SelectAllCompaniesAsync()
    {
        if (_transaction?.Connection == null)
            throw new NullReferenceException("Database connection or not established.");

        var companies = await _transaction.Connection.QueryAsync<Models.Company>(CompanyQueries.SelectAll, _transaction);

        return companies.Select(_ => new Company(
                new IdValue<int>(_.Id.GetValueOrDefault()),
                new CompanyCode(_.Code),
                new CompanyName(_.Name),
                new Country(_.CountryCode)
            )
        );
    }
}