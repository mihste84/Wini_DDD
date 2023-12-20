using System.Text.RegularExpressions;

namespace Domain.Wini.Validators;
public class SubledgerValidator : AbstractValidator<Subledger>
{
    public SubledgerValidator(bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Value), () => RuleFor(_ => _.Value).Must(_ => _?.Contains(';') == false));
        When(_ => !string.IsNullOrWhiteSpace(_.Type), () => RuleFor(_ => _.Type).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Value).MaximumLength(8).WithName("Subledger");
        RuleFor(_ => _.Type).MaximumLength(1).WithName("Subledger Type");

        When(_ => isRequired && !string.IsNullOrWhiteSpace(_.Value), () =>

            RuleFor(_ => _.Type)
                .NotEmpty().WithMessage("Subledger Type must be entered if Subledger is to be used.")
                .Matches("A", RegexOptions.IgnoreCase).WithMessage("Subledger Type must be 'A'.")
                .WithName("Subledger Type")
        );
        When(_ => isRequired && !string.IsNullOrWhiteSpace(_.Type), () =>
            RuleFor(_ => _.Value)
                .NotEmpty()
                .WithMessage("Subledger cannot be empty when Subledger Type has a value.")
                .WithName("Subledger")
        );
    }
}