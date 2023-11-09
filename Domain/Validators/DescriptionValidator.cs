namespace Domain.Validators;
public class DescriptionValidator : AbstractValidator<Description>
{
    public DescriptionValidator(string name)
    {
        RuleFor(_ => _.Message).MaximumLength(30).WithName(name);
    }
}