namespace Domain.Values;

public record Comment
{
    public readonly IdValue<int> BookingId;
    public string Value { get; }
    public DateTime Created { get; set; }

    public Comment(string value, IdValue<int> bookingId)
    {
        if (value.Length > 300)
        {
            throw new TextValidationException(
                nameof(value),
                value,
                ValidationErrorCodes.TextTooLong,
                "Comment cannot be longer than 300 characters"
            )
            { MaxLength = 300 };
        }

        Value = value;
        BookingId = bookingId;
        Created = DateTime.UtcNow;
    }
}