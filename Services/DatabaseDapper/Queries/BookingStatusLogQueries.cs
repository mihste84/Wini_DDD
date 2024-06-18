namespace DatabaseDapper.Queries;

public static class BookingStatusLogQueries
{
    public const string Insert = """
        INSERT INTO dbo.BookingStatusLogs (
            BookingId,
            Status,
            Created,
            CreatedBy
        )
        VALUES(
            @BookingId,
            @Status,         
            @Created,
            @CreatedBy
        )
    """;
}