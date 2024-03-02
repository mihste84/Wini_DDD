namespace Services.DatabaseDapper.Queries;

public static class CommentQueries
{
    public const string SelectByBookingId = """
        SELECT * FROM dbo.Comments
        WHERE BookingId = @BookingId
    """;

    public const string Insert = """
        INSERT INTO dbo.Comments (
            BookingId,
            Value,
            Created,
            CreatedBy
        )
        VALUES(
            @BookingId,
            @Value,         
            @Created,
            @CreatedBy
        )
    """;

    public const string DeleteAllByBookingId = """
        DELETE FROM dbo.Comments
        WHERE BookingId = @BookingId      
    """;
}