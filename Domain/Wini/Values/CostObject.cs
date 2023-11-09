namespace Domain.Wini.Values;

public record CostObject
{
    public string? Value { get; }
    public string? Type { get; }
    public readonly int Number;

    public CostObject(int number, string? costObject, string? type)
    {
        if (!(number is >= 1 and <= 4))
        {
            throw new NumberValidationException(
                nameof(number),
                number,
                ValidationErrorCodes.OutOfRange,
                "Cost object number must be between 1 and 4"
            )
            { MaxValue = 4, MinValue = 1 };
        }

        if (!string.IsNullOrWhiteSpace(costObject) && costObject.Length > 12)
        {
            throw new TextValidationException(
                nameof(costObject),
                costObject,
                ValidationErrorCodes.TextTooLong,
                "Cost object cannot be longer than 12 characters"
            )
            { MaxLength = 12 };
        }

        if (!string.IsNullOrWhiteSpace(type) && type.Length > 1)
        {
            throw new TextValidationException(
                nameof(type),
                type,
                ValidationErrorCodes.TextTooLong,
                "Cost object type cannot be longer than 1 character"
            )
            { MaxLength = 1 };
        }

        Value = costObject;
        Type = type;
        Number = number;
    }
}