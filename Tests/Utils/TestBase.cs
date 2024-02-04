using Microsoft.Extensions.Configuration;
using Respawn;
using Respawn.Graph;

namespace Tests.Utils;

public class TestBase
{
    public readonly HttpClient HttpClient;
    private readonly string _connectionString;
    public readonly IMasterdataRepository MasterdataRepository;
    public TestBase()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var configuration = webApplicationFactory.Services.GetService(typeof(IConfiguration)) as IConfiguration;
        _connectionString = configuration!.GetConnectionString("WiniDb")!;
        HttpClient = webApplicationFactory.CreateDefaultClient();
        var connectionFactory = new ConnectionFactory(_connectionString);
        MasterdataRepository = new MasterdataRepository(connectionFactory);
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
}