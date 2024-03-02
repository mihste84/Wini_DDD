namespace Services.DatabaseDapper.Queries;

public static class RecipientMessageQueries
{
    public const string SelectByBookingId = """
        SELECT * FROM dbo.RecipientMessages
        WHERE BookingId = @BookingId
    """;

    public const string Insert = """
        INSERT INTO dbo.RecipientMessages (
            BookingId,
            Value,
            Recipient,
            Created,
            CreatedBy
        )
        VALUES(
            @BookingId,
            @Value,
            @Recipient,  
            GETUTCDATE(),
            @CreatedBy
        )
    """;

    public const string DeleteAllByBookingId = """
        DELETE FROM dbo.RecipientMessages
        WHERE BookingId = @BookingId      
    """;
}