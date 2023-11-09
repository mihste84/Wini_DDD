namespace Domain.Values;

public record BookingRowNumber
{
    public int? Number { get; }
    public BookingRowNumber(int? number)
    {
        if (number.HasValue && number < 1)
        {
            throw new NumberValidationException(
                nameof(number),
                number,
                ValidationErrorCodes.ValueTooLow,
                "Row number must be greater than 1"
            )
            { MinValue = 1 };
        }

        Number = number;
    }
}