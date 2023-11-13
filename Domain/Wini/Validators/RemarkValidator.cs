namespace Domain.Wini.Validators;
public class RemarkValidator : AbstractValidator<Remark>
{
    public RemarkValidator()
    {
        RuleFor(_ => _.Text).MaximumLength(30);
    }
}