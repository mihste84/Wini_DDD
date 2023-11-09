namespace Domain.Values;

public record Costcenter
{
    public string? Code { get; }

    public Costcenter(string? costcenter)
    {
        if (!string.IsNullOrWhiteSpace(costcenter) && costcenter.Length > 5)
        {
            throw new TextValidationException(
                nameof(costcenter),
                costcenter,
                ValidationErrorCodes.TextTooLong,
                "Costcenter cannot be longer than 5 characters"
            )
            { MaxLength = 5 };
        }

        Code = costcenter;
    }
}