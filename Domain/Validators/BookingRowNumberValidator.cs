namespace Domain.Validators;
public class BookingRowNumberValidator : AbstractValidator<BookingRowNumber>
{
    public BookingRowNumberValidator()
    {
        RuleFor(_ => _.Number).NotEmpty().GreaterThanOrEqualTo(1);
    }
}