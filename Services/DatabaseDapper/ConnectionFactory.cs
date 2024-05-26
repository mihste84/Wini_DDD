namespace Services.DatabaseDapper;

public class ConnectionFactory(string connectionString)
{
    public IDbConnection CreateConnection()
    => new SqlConnection(connectionString);
}
