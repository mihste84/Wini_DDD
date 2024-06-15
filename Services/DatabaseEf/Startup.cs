using DatabaseEf;
using Services.DatabaseCommon;
using Microsoft.EntityFrameworkCore;
using Services.DatabaseEf.Repositories;

namespace Services.DatabaseEf;

public static class Startup
{
    public static void AddDatabaseEfServices(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be empty.");
        }

        services.AddScoped<IMasterdataRepository, MasterdataRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddDbContext<WiniDbContext>((o) => o.UseSqlServer(connectionString, x => x.EnableRetryOnFailure()));
        services.AddScoped<ITransactionScope, TransactionScopeWrapper>();
        services.AddScoped<ITransactionScopeManager, TransactionScopeManager>();
    }
}