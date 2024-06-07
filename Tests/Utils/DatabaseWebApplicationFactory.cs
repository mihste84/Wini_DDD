using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Services.DatabaseDapper;

namespace Tests.Utils;

public class DatabaseWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(service => typeof(ConnectionFactory) == service.ServiceType));
            services.AddSingleton(_ => new ConnectionFactory(connectionString));
        });
    }
}