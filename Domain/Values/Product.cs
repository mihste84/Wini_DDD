namespace Domain.Values;

public record Product
{
    public string? Code { get; }

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