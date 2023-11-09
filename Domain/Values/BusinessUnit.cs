namespace Domain.Values;

public record BusinessUnit
{
    public CompanyCode CompanyCode { get; }
    public Costcenter Costcenter { get; }
    public Product Product { get; }

    public BusinessUnit(string businessUnit)
    {
        if (!string.IsNullOrWhiteSpace(businessUnit) && businessUnit.Length > 12)
        {
            throw new TextValidationException(
                nameof(businessUnit),
                businessUnit,
                ValidationErrorCodes.TextTooLong,
                "BusinessUnit cannot be longer than 12 characters"
            )
            { MaxLength = 12 };
        }

        businessUnit = businessUnit.Trim();
        CompanyCode = GetCompanyCode(businessUnit);
        Costcenter = GetCostcenter(businessUnit);
        Product = GetProduct(businessUnit);
    }

    private static Costcenter GetCostcenter(string businessUnit)
        => (businessUnit?.Length >= 8)
        ? new Costcenter(businessUnit[3..8])
        : new Costcenter("");

    private static CompanyCode GetCompanyCode(string businessUnit)
        => (businessUnit?.Length >= 3)
        ? new CompanyCode(businessUnit[..3])
        : new CompanyCode("");

    private static Product GetProduct(string businessUnit)
        => (businessUnit?.Length >= 12)
        ? new Product(businessUnit[8..12])
        : new Product("");

    public override string ToString() => $"{CompanyCode.Code}{Costcenter.Code}{Product.Code}";
}