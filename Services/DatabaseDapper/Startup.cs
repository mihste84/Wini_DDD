namespace Services.DatabaseDapper;

public static class Startup
{
    public static void AddDatabaseServices(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be empty.");

        services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));
        services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
    }
}