namespace Domain.Wini.Validators;
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Code), () => RuleFor(_ => _.Code).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Code).MaximumLength(4).WithName("Product");
    }
}