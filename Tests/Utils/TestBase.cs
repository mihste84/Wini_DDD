using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Utils;

public class TestBase : IDisposable
{
    public readonly HttpClient HttpClient;
    private readonly string _connectionString;
    public readonly DapperUnitOfWork UnitOfWork;
    public TestBase()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var configuration = webApplicationFactory.Services.GetService(typeof(IConfiguration)) as IConfiguration;
        _connectionString = configuration!.GetConnectionString("WiniDb")!;
        HttpClient = webApplicationFactory.CreateDefaultClient();
        var loggerMock = new Mock<ILogger<DapperUnitOfWork>>();
        var connection = new SqlConnection(_connectionString);
        UnitOfWork = new DapperUnitOfWork(connection, loggerMock.Object);
    }

    private static async Task<Respawner> GetRespawnAsync(string connectionString)
    => await Respawner.CreateAsync(connectionString, new RespawnerOptions
    {
        TablesToIgnore = new Table[]
        {
            "Companies"
        }
    });

    public async Task ResetDbAsync()
    {
        var respawn = await GetRespawnAsync(_connectionString);
        await respawn.ResetAsync(_connectionString);
    }

    public void Dispose()
    {
        UnitOfWork.Dispose();
        GC.SuppressFinalize(this);
    }
}