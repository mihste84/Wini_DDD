using Microsoft.Extensions.Configuration;
using Respawn;
using Respawn.Graph;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Tests.Utils;

public class TestBase
{
    public readonly HttpClient HttpClient;
    private readonly string _connectionString;

    public TestBase()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var configuration = webApplicationFactory.Services.GetService(typeof(IConfiguration)) as IConfiguration;
        _connectionString = configuration!.GetConnectionString("WiniDb")!;
        HttpClient = webApplicationFactory.CreateDefaultClient();
    }

    private static Task<Respawner> GetRespawnAsync(string connectionString)
    => Respawner.CreateAsync(
        connectionString,
        new RespawnerOptions
        {
            TablesToIgnore = [
                "Companies"
            ]
        });

    public async Task<SqlResult?> InsertAsync<M>(string sql, M model)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleAsync<SqlResult>(sql, model);
    }

    public async Task<int?> InsertMultipleAsync<M>(string sql, M model)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.ExecuteAsync(sql, model);
    }

    public async Task<IEnumerable<M>> QueryAsync<M>(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QueryAsync<M>(sql, model);
    }

    public async Task<M?> QuerySingleAsync<M>(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<M>(sql, model);
    }

    public async Task ExecuteAsync(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await conn.ExecuteAsync(sql, model);
    }

    public async Task ResetDbAsync()
    {
        var respawn = await GetRespawnAsync(_connectionString);
        await respawn.ResetAsync(_connectionString);
    }

    public async Task<SqlResult> SeedBaseBookingAsync(
        Services.DatabaseDapper.Models.Booking? booking,
        Services.DatabaseDapper.Models.BookingRow[]? rows,
        Services.DatabaseDapper.Models.Comment[]? comments = default,
        Services.DatabaseDapper.Models.RecipientMessage[]? messages = default
    )
    {
        var bookingModel = booking
            ?? new Services.DatabaseDapper.Models.Booking
            {
                BookingDate = new DateTime(2024, 1, 1),
                Created = DateTime.UtcNow,
                CreatedBy = TestAuthenticationService.UserId,
                LedgerType = (short)Ledgers.GP,
                Reversed = true,
                Status = (short)WiniStatus.Saved,
                TextToE1 = "Test",
                Updated = DateTime.UtcNow,
                UpdatedBy = TestAuthenticationService.UserId
            };
        var sqlResult = await InsertAsync(BookingQueries.Insert, bookingModel);
        Assert.NotNull(sqlResult);
        var rowToInsert = rows?
            .Select(_ =>
            {
                _.BookingId = sqlResult.Id;
                return _;
            })
            ?? [
            new Services.DatabaseDapper.Models.BookingRow
            {
                RowNumber = 1,
                Account = "12345",
                Amount = 100,
                Authorizer = "MIHSTE@mail.com",
                BusinessUnit = "100KKTOT",
                CostObject1 = "CO1",
                CostObject2 = "CO2",
                CostObject3 = "CO3",
                CostObject4 = "CO4",
                CostObjectType1 = "1",
                CostObjectType2 = "2",
                CostObjectType3 = "3",
                CostObjectType4 = "4",
                CurrencyCode = "NOK",
                ExchangeRate = 1.2m,
                Subledger = "XYZ",
                SubledgerType = "A",
                Remark = "TEST BOOKING ROW",
                Subsidiary = "1234",
                BookingId = sqlResult.Id,
                IsAuthorized = false
            }
        ];

        await InsertMultipleAsync(BookingRowQueries.Insert, rowToInsert);

        if (comments != default)
        {
            await InsertMultipleAsync(
                CommentQueries.Insert,
                comments.Select(_ =>
                {
                    _.BookingId = sqlResult.Id;
                    return _;
                }));
        }

        if (messages != default)
        {
            await InsertMultipleAsync(
                RecipientMessageQueries.Insert,
                messages.Select(_ =>
                {
                    _.BookingId = sqlResult.Id;
                    return _;
                }));
        }

        return sqlResult;
    }
}