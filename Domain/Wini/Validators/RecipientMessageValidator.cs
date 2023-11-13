namespace Domain.Wini.Validators;
public class RecipientMessageValidator : AbstractValidator<RecipientMessage>
{
    public RecipientMessageValidator()
    {
        RuleFor(_ => _.BookingId).NotNull();
        RuleFor(_ => _.Recipient).NotNull();
        RuleFor(_ => _.Message).NotEmpty().MaximumLength(100);
    }
}