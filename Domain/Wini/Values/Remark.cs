namespace Domain.Wini.Values;

public record Remark
{
    public string? Text { get; }

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

    public Remark Copy() => new(Text);
}