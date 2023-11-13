namespace Domain.Wini.Values;

public record Comment
{
    public readonly IdValue<int> BookingId;
    public string Value { get; }
    public DateTime Created { get; set; }

    public Comment(string value, IdValue<int> bookingId)
    {
        Value = value;
        BookingId = bookingId;
        Created = DateTime.UtcNow;

        var validator = new CommentValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}