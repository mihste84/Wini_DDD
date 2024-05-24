namespace Domain.Common.Values;

public readonly record struct CompanyName
{
    public readonly string Name;
    public CompanyName(string? name)
    {
        Name = name!;

        var validator = new CompanyNameValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}