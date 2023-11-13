namespace Domain.Values;

public record Remark
{
    public string? Text { get; }
    public Remark(string? text)
    {
        Text = text;

        var validator = new RemarkValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new DomainValidationException(result.Errors);
    }
}