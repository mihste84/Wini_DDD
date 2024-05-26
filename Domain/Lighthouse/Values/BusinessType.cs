
namespace Domain.Lighthouse.Values;

public readonly record struct BusinessType
{
    public readonly string? Code;

    public BusinessType()
    {
    }

    public BusinessType(string? code)
    {
        Code = code;

        var validator = new BusinessTypeValidator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}