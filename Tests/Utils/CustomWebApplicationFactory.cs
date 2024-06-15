using System.Data.Common;
using AppLogic.WiniLogic.Interfaces;
using DatabaseEf;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DatabaseDapper;
using Tests.MockServices;

namespace Tests.Utils;

public class CustomWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            TryReplaceService<IAuthenticationService>(services, _ => _.AddScoped<IAuthenticationService, TestAuthenticationService>());
            TryReplaceService<IAuthorizationService>(services, _ => _.AddScoped<IAuthorizationService, TestAuthorizationService>());
            TryReplaceService<IAuthorizerValidationService>(services, _ => _.AddSingleton<IAuthorizerValidationService, TestAuthorizerValidationService>());
            TryReplaceService<IBookingPeriodValidationService>(services, _ => _.AddSingleton<IBookingPeriodValidationService, TestBookingPeriodValidationService>());
            TryReplaceService<IAccountingValidationService>(services, _ => _.AddSingleton<IAccountingValidationService, TestAccountingValidationService>());
            TryReplaceService<IAttachmentService>(services, _ => _.AddScoped<IAttachmentService, TestAttachmentService>());
            TryReplaceService<IPolicyEvaluator>(services, _ => _.AddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>());

            if (Environment.GetEnvironmentVariable("DB_TYPE") == "Ef")
            {
                TryReplaceService<DbContextOptions<WiniDbContext>>(services, _ => _.AddDbContext<WiniDbContext>((_, option) => option.UseSqlServer(connectionString)));
            }
            else if (Environment.GetEnvironmentVariable("DB_TYPE") == "Dapper")
            {
                TryReplaceService<DbConnection>(services, _ => _.AddSingleton(_ => new ConnectionFactory(connectionString)));
            }
        });
    }

    private static void TryReplaceService<T>(IServiceCollection services, Action<IServiceCollection> func)
    {
        var service = services.SingleOrDefault(service => typeof(T) == service.ServiceType);
        if (service != default) services.Remove(service);
        func(services);
    }
}