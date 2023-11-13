namespace Domain.Wini.Values;

public record BookingRowNumber
{
    public int? Number { get; }
    public BookingRowNumber(int? number)
    {
        Number = number;
        var validator = new BookingRowNumberValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}