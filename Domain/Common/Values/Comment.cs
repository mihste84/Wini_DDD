namespace Domain.Common.Values;

public record Comment
{
    public readonly IdValue<int> BookingId;
    public readonly string? Value;
    public readonly DateTime Created;

    public Comment(string? value, IdValue<int> bookingId, DateTime created)
    {
        Value = value;
        BookingId = bookingId;
        Created = created;

        var validator = new CommentValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }

    public Comment(string? value, IdValue<int> bookingId)
    {
        Value = value;
        BookingId = bookingId;
        Created = DateTime.UtcNow;

        var validator = new CommentValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}