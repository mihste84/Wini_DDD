namespace Domain.Wini.Values;

public record TextToE1
{
    public string? Text { get; }
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