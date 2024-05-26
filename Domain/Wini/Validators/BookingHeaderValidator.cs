namespace Domain.Wini.Validators;
public class BookingHeaderValidator : AbstractValidator<BookingHeader>
{
    public BookingHeaderValidator()
    {
        RuleFor(_ => _.BookingDate.Date).NotEmpty();
        RuleFor(_ => _.TextToE1).SetValidator(new TextToE1Validator());
    }
}