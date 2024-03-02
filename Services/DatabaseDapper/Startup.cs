namespace Services.DatabaseDapper;

public static class Startup
{
    public static void AddDatabaseServices(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be empty.");

        services.AddSingleton(_ => new ConnectionFactory(connectionString));
        services.AddScoped<IMasterdataRepository, MasterdataRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ITransactionScope, TransactionScopeWrapper>();
        services.AddScoped<ITransactionScopeManager, TransactionScopeManager>();
    }
}