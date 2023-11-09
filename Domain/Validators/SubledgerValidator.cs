using System.Text.RegularExpressions;

namespace Domain.Validators;
public class SubledgerValidator : AbstractValidator<Subledger>
{
    public SubledgerValidator()
    {
        RuleFor(_ => _.Value).MaximumLength(8);
        When(_ => !string.IsNullOrEmpty(_.Value), () =>
        {
            RuleFor(_ => _.Type)
                .NotEmpty().WithMessage("Subledger Type must be entered if Subledger is to be used.")
                .Matches("A", RegexOptions.IgnoreCase).WithMessage("Subledger Type must be 'A'.")
                .WithName("Subledger Type");
        });
        RuleFor(_ => _.Value).MaximumLength(1).WithName("Subledger Type");
        When(_ => !string.IsNullOrEmpty(_.Value), () =>
        {
            RuleFor(_ => _.Value).NotEmpty().WithMessage("Subledger cannot be empty when Subledger Type has a value.");
        });
    }
}