using Services.DatabaseCommon;

namespace Services.DatabaseEf;

public static class Startup
{
    public static void AddDatabaseEfServices(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be empty.");
        }

        // services.AddScoped<IMasterdataRepository, MasterdataRepository>();
        // services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ITransactionScope, TransactionScopeWrapper>();
        services.AddScoped<ITransactionScopeManager, TransactionScopeManager>();
    }
}