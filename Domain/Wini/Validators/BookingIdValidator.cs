namespace Domain.Wini.Validators;
public class BookingIdValidator : AbstractValidator<IdValue<int>>
{
    public BookingIdValidator()
    {
        RuleFor(_ => _.Value).NotEmpty();
    }
}