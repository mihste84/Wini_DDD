namespace Domain.Wini.Validators;
public class BookingIdValidator : AbstractValidator<IdValue<int>?>
{
    public BookingIdValidator()
    {
        When(_ => _ != default, () => RuleFor(_ => _!.Value).NotEmpty());
    }
}