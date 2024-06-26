namespace Domain.Wini.Validators;
public class CostObjectValidator : AbstractValidator<CostObject>
{
    public CostObjectValidator(int number, bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Type), () => RuleFor(_ => _.Type).Must(_ => _?.Contains(';') == false));
        When(_ => !string.IsNullOrWhiteSpace(_.Value), () => RuleFor(_ => _.Value).Must(_ => _?.Contains(';') == false));

        RuleFor(_ => _.Number).InclusiveBetween(1, 4).Equal(number).WithName("Cost Object Number");
        RuleFor(_ => _.Type).MaximumLength(1).WithName(_ => $"Cost Object Type {_.Number}");
        RuleFor(_ => _.Value).MaximumLength(12).WithName(_ => $"Cost Object {_.Number}");

        When(
            _ => isRequired && !string.IsNullOrWhiteSpace(_.Type),
            () =>
            {
                RuleFor(_ => _.Value)
                    .NotEmpty()
                    .WithMessage(_ => $"Cost Object {_.Number} must be entered if Cost Object Type {_.Number} is to be used.")
                    .WithName(_ => $"Cost Object {_.Number}");
            });

        When(
            _ => isRequired && !string.IsNullOrWhiteSpace(_.Value),
            () =>
            {
                RuleFor(_ => _.Type)
                    .NotEmpty()
                    .WithMessage(_ => $"Cost Object Type {_.Number} must be entered if Cost Object {_.Number} is used.")
                    .WithName(_ => $"Cost Object Type {_.Number}");
            });
    }
}