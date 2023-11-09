namespace Domain.Validators;
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(_ => _.Code).MaximumLength(4).WithName("Product");
    }
}