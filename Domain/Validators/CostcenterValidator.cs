namespace Domain.Validators;
public class CostcenterValidator : AbstractValidator<Costcenter>
{
    public CostcenterValidator()
    {
        RuleFor(_ => _.Code).NotEmpty().MaximumLength(5).WithName("Costcenter");
    }
}