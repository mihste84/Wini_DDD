namespace Domain.Values;

public record BusinessUnit
{
    public CompanyCode CompanyCode { get; }
    public Costcenter Costcenter { get; }
    public Product Product { get; }

    public BusinessUnit(string businessUnit)
    {
        businessUnit = businessUnit.Trim();
        CompanyCode = GetCompanyCode(businessUnit);
        Costcenter = GetCostcenter(businessUnit);
        Product = GetProduct(businessUnit);

        var validator = new BusinessUnitValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
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
        ? new Product(businessUnit[8..])
        : new Product("");

    public override string ToString() => $"{CompanyCode.Code}{Costcenter.Code}{Product.Code}";
}