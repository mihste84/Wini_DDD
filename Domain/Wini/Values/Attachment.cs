namespace Domain.Wini.Values;

public record Attachment
{
    public readonly BookingType Type;
    public readonly IdValue<int> BookingId;
    public readonly FileContent Content;

    public Attachment(BookingType type, IdValue<int> bookingId, FileContent content)
    {
        Type = type;
        BookingId = bookingId;
        Content = content;
    }
}