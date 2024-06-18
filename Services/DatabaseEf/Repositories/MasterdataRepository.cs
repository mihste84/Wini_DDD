using Domain.Common.Entities;
using Domain.Common.Values;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEf.Repositories;

public class MasterdataRepository(WiniDbContext ctx) : IMasterdataRepository
{
    public async Task<IEnumerable<Company>> SelectAllCompaniesAsync()
    {
        var companies = await ctx.Companies
            .AsNoTracking()
            .ToListAsync();

        return companies
            .Select(_ => new Company(
                new IdValue<int>(_.Id),
                new CompanyCode(_.Code),
                new CompanyName(_.Name),
                new Country(_.CountryCode)
                )
            );
    }
}