namespace Services.DatabaseDapper.Repositories;

public class MasterdataRepository(ConnectionFactory factory) : IMasterdataRepository
{
    public async Task<IEnumerable<Company>> SelectAllCompaniesAsync()
    {
        using var conn = factory.CreateConnection();
        conn.Open();
        var companies = await conn.QueryAsync<DatabaseCommon.Models.Company>(CompanyQueries.SelectAll);

        return companies.Select(_ => new Company(
            new IdValue<int>(_.Id),
            new CompanyCode(_.Code),
            new CompanyName(_.Name),
            new Country(_.CountryCode)
            )
        );
    }
}