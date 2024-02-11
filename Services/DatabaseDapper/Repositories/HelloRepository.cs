using AppLogic.Models;

namespace Services.DatabaseDapper.Repositories;

public class HelloRepository : IHelloRepository
{
    private readonly ConnectionFactory _factory;
    private const string _insert =
    """
        INSERT INTO HelloWorld (Text, CreatedBy, UpdatedBy)
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        VALUES (@Text, @CreatedBy, @UpdatedBy);
    """;

    private const string _update =
    """
        UPDATE HelloWorld 
        SET Text = @Text,
            UpdatedBy = @UpdatedBy,
            Updated = @Updated
        OUTPUT INSERTED.[Id], INSERTED.RowVersion 
        WHERE Id = @Id
    """;

    private const string _selectRowVersionById =
    """
        SELECT RowVersion FROM HelloWorld WHERE Id = @Id
    """;

    private const string _delete = "DELETE FROM HelloWorld WHERE Id = @Id";
    public HelloRepository(ConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> IsSameRowVersion(int id, byte[] rowVersion)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        var res = await conn.QueryFirstAsync<byte[]>(_selectRowVersionById, new { Id = id });

        return rowVersion.SequenceEqual(res);
    }

    public async Task<SqlResult?> InsertHelloTextAsync(string text, string createdBy)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        return await conn.QuerySingleAsync<SqlResult>(_insert, new { Text = text, CreatedBy = createdBy, UpdatedBy = createdBy });
    }

    public async Task<SqlResult> UpdateAsync(int id, string text, byte[] rowVersion, string updatedBy)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();
        var updateObj = new { Id = id, Text = text, RowVersion = rowVersion, UpdatedBy = updatedBy, Updated = DateTime.UtcNow };
        return await conn.QuerySingleAsync<SqlResult>(_update, updateObj);
    }

    public async Task<IEnumerable<string>> SelectAllAsync()
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        return await conn.QueryAsync<string>("SELECT Text FROM dbo.HelloWorld");
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();

        return (await conn.ExecuteAsync(_delete, new { Id = id })) > 0;
    }
}