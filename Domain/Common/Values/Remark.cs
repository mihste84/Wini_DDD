namespace Domain.Common.Values;

public readonly record struct Remark
{
    public readonly string? Text;

    public Remark()
    {
    }
    public Remark(string? text)
    {
        Text = text;

        var validator = new RemarkValidator();
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}