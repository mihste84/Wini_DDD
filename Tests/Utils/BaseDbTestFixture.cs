using Respawn;
using Dapper;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using DatabaseDapper.Queries;
using Tests.MockServices;

namespace Tests.Utils;

public class BaseDbTestFixture : IAsyncLifetime
{
    public readonly MsSqlContainer Container;

    public BaseDbTestFixture()
    {
        Container = new MsSqlBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();
    }

    public async Task<SqlResult?> InsertAsync<M>(string sql, M model)
    {
        await using var conn = new SqlConnection(GetConnectionString());
        await conn.OpenAsync();

        return await conn.QuerySingleAsync<SqlResult>(sql, model);
    }

    public async Task<int?> InsertMultipleAsync<M>(string sql, M model)
    {
        await using var conn = new SqlConnection(GetConnectionString());
        await conn.OpenAsync();

        return await conn.ExecuteAsync(sql, model);
    }

    public async Task<IEnumerable<M>> QueryAsync<M>(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(GetConnectionString());
        await conn.OpenAsync();

        return await conn.QueryAsync<M>(sql, model);
    }

    public async Task<M?> QuerySingleAsync<M>(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(GetConnectionString());
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<M>(sql, model);
    }

    public async Task ExecuteAsync(string sql, object? model = default)
    {
        await using var conn = new SqlConnection(GetConnectionString());
        await conn.OpenAsync();

        await conn.ExecuteAsync(sql, model);
    }

    public async Task ResetDbAsync()
    {
        var respawn = await GetRespawnAsync(GetConnectionString());
        await respawn.ResetAsync(GetConnectionString());
    }

    public async Task<SqlResult> SeedBaseBookingAsync(
        DatabaseCommon.Models.Booking? booking,
        DatabaseCommon.Models.BookingRow[]? rows,
        DatabaseCommon.Models.Comment[]? comments = default,
        DatabaseCommon.Models.RecipientMessage[]? messages = default,
        DatabaseCommon.Models.Attachment[]? attachments = default
    )
    {
        var bookingModel = booking
            ?? new DatabaseCommon.Models.Booking
            {
                BookingDate = new DateOnly(2024, 1, 1).ToDateTime(default),
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
            new DatabaseCommon.Models.BookingRow
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

        if (attachments != default)
        {
            await InsertMultipleAsync(
                AttachmentQueries.Insert,
                attachments.Select(_ =>
                {
                    _.BookingId = sqlResult.Id;
                    return _;
                }));
        }

        return sqlResult;
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        using var conn = new SqlConnection(Container.GetConnectionString());
        await conn.OpenAsync();
        await conn.ExecuteAsync("CREATE DATABASE WiniDb");
        conn.ChangeDatabase("WiniDb");

        var baseFolder = Environment.CurrentDirectory;
        var tablesPath = Path.Combine(baseFolder, "Tables");
        var seedPath = Path.Combine(baseFolder, "PostDeployment");

        string[] tablesFiles = [
            Path.Combine(tablesPath, "Bookings.sql"),
            Path.Combine(tablesPath, "BookingRows.sql"),
            Path.Combine(tablesPath, "BookingLogs.sql"),
            Path.Combine(tablesPath, "Companies.sql"),
            Path.Combine(tablesPath, "Attachments.sql"),
            Path.Combine(tablesPath, "Comments.sql"),
            Path.Combine(tablesPath, "RecipientMessages.sql")
        ];

        foreach (var file in tablesFiles)
        {
            var sql = File.ReadAllText(file);

            foreach (var item in sql.Split("GO").Where(_ => !string.IsNullOrWhiteSpace(_)))
            {
                await conn.ExecuteAsync(item);
            }
        }

        var seedFiles = Directory.GetFiles(seedPath);
        foreach (var file in seedFiles)
        {
            var sql = File.ReadAllText(file);
            foreach (var item in sql.Split("GO"))
            {
                await conn.ExecuteAsync(item);
            }
        }
    }

    public Task DisposeAsync()
    => Container.DisposeAsync().AsTask();

    public string GetConnectionString() => Container.GetConnectionString().Replace("Database=master", "Database=WiniDb");

    private static Task<Respawner> GetRespawnAsync(string connectionString)
    => Respawner.CreateAsync(
        connectionString,
        new RespawnerOptions
        {
            TablesToIgnore = [
                "Companies"
            ]
        });
}
