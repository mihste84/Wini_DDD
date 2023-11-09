namespace Domain.Values;

public record Product
{
    public string? Code { get; }

    public Product(string? product)
    {
        if (!string.IsNullOrWhiteSpace(product) && product.Length > 4)
        {
            throw new TextValidationException(
                nameof(product),
                product,
                ValidationErrorCodes.TextTooLong,
                "Product cannot be longer than 4 characters"
            )
            { MaxLength = 4 };
        }

        Code = product;
    }
}