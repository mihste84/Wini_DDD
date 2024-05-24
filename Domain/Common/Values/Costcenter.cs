namespace Domain.Common.Values;

public readonly record struct Costcenter
{
    public readonly string? Code;

    public Costcenter()
    {
    }

    public Costcenter(string? costcenter)
    {
        Code = costcenter;

        var validator = new CostcenterValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}