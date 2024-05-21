namespace Domain.Validators;
public class CostcenterValidator : AbstractValidator<Costcenter>
{
    public CostcenterValidator(bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Code), () => RuleFor(_ => _.Code).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Code)
            .MaximumLength(5)
            .WithName("Costcenter");

        When(
            _ => isRequired,
            () =>
            {
                RuleFor(_ => _.Code)
                    .NotEmpty()
                    .WithName("Costcenter");
            });
    }
}