namespace Domain.Values;

public record Costcenter
{
    public string? Code { get; }

    public Costcenter(string? costcenter)
    {
        Code = costcenter;

        var validator = new CostcenterValidator(false);
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}