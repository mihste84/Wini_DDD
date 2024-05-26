namespace Domain.Common.Validators;
public class RemarkValidator : AbstractValidator<Remark>
{
    public RemarkValidator()
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Text), () => RuleFor(_ => _.Text).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Text).MaximumLength(30);
    }
}