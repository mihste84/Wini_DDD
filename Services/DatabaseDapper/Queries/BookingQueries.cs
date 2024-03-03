namespace Services.DatabaseDapper.Queries;

public static class BookingQueries
{
    public const string DeleteBookingById = """
        DELETE FROM dbo.Bookings
        WHERE Id = @BookingId;
    """;

    public const string SelectBookingById = """
        SELECT TOP 1 * FROM dbo.Bookings
        WHERE Id = @BookingId;

        SELECT * FROM dbo.Comments
        WHERE BookingId = @BookingId;

        SELECT * FROM dbo.RecipientMessages
        WHERE BookingId = @BookingId;

        SELECT * FROM dbo.BookingStatusLogs
        WHERE BookingId = @BookingId;

        SELECT * FROM dbo.Attachments
        WHERE BookingId = @BookingId;
    """;

    public const string SelectBookingAndRowsById = SelectBookingById
        + """
        
        SELECT * FROM dbo.BookingRows
        WHERE BookingId = @BookingId AND IsDeleted = 0
        ORDER BY RowNumber;
    """;

    public const string Insert = """
        INSERT INTO dbo.Bookings (Status, BookingDate, TextToE1, Reversed, LedgerType, UpdatedBy, CreatedBy)
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        VALUES(@Status, @BookingDate, @TextToE1, @Reversed, @LedgerType, @UpdatedBy, @CreatedBy)
    """;

    public const string SelectRowVersionById =
    """
        SELECT RowVersion FROM dbo.Bookings WHERE Id = @Id
    """;

    public const string Update = """
        UPDATE dbo.Bookings
        SET BookingDate = @BookingDate,
            TextToE1 = @TextToE1,
            Reversed = @Reversed,
            LedgerType = @LedgerType,
            UpdatedBy = @UpdatedBy
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        WHERE Id = @Id
    """;

    public const string UpdateStatus = """
        UPDATE dbo.Bookings
        SET Status = @Status,
            UpdatedBy = @UpdatedBy
        OUTPUT INSERTED.[Id], INSERTED.RowVersion
        WHERE Id = @Id
    """;
}