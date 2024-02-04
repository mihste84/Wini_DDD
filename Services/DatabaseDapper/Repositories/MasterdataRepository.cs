namespace Services.DatabaseDapper.Repositories;

public class MasterdataRepository : IMasterdataRepository
{
    private readonly ConnectionFactory _factory;

    public MasterdataRepository(ConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Company>> SelectAllCompaniesAsync()
    {
        using var conn = _factory.CreateConnection();
        conn.Open();
        var companies = await conn.QueryAsync<Models.Company>(CompanyQueries.SelectAll);

        return companies.Select(_ => new Company(
                new IdValue<int>(_.Id.GetValueOrDefault()),
                new CompanyCode(_.Code),
                new CompanyName(_.Name),
                new Country(_.CountryCode)
            )
        );
    }
}