namespace Domain.Values;

public record Subledger
{
    public string? Value { get; }
    public string? Type { get; }

    public Subledger(string? subledger, string? type)
    {
        if (!string.IsNullOrWhiteSpace(subledger) && subledger.Length > 8)
        {
            throw new TextValidationException(
                nameof(subledger),
                subledger,
                ValidationErrorCodes.TextTooLong,
                "Subledger cannot be longer than 8 characters"
            )
            { MaxLength = 8 };
        }

        if (!string.IsNullOrWhiteSpace(type) && type.Length > 1)
        {
            throw new TextValidationException(
                nameof(type),
                type,
                ValidationErrorCodes.TextTooLong,
                "Subledger type cannot be longer than 1 characters"
            )
            { MaxLength = 1 };
        }

        Value = subledger;
        Type = type;
    }
}