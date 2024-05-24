namespace Domain.Common.Values;

public readonly record struct Product
{
    public readonly string? Code;
    public Product()
    {
    }

    public Product(string? product)
    {
        Code = product;

        var validator = new ProductValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}