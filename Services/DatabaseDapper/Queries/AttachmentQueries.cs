namespace Services.DatabaseDapper.Queries;

public static class AttachmentQueries
{
    public const string SelectByBookingId = """
        SELECT * FROM dbo.Attachments
        WHERE BookingId = @BookingId
    """;

    public const string Insert = """
        INSERT INTO dbo.Attachments (
            BookingId
            ,Size
            ,ContentType
            ,Name
            ,Path
            ,CreatedBy
        )
        VALUES(
            @BookingId
            ,@Size
            ,@ContentType
            ,@Name
            ,@Path
            ,@CreatedBy
        )
    """;

    public const string DeleteByBookingName = """
        DELETE FROM dbo.Attachments
        WHERE BookingId = @BookingId AND Name IN @Names
    """;
}