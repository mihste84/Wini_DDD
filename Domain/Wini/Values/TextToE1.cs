namespace Domain.Wini.Values;

public readonly record struct TextToE1
{
    public readonly string? Text;
    public TextToE1()
    {
    }
    public TextToE1(string? text)
    {
        Text = text;

        var validator = new TextToE1Validator(false);
        var result = validator.Validate(this);
        if (result.IsValid)
        {
            return;
        }

        throw new DomainValidationException(result.Errors);
    }
}