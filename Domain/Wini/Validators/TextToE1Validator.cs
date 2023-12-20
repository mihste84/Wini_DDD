namespace Domain.Wini.Validators;
public class TextToE1Validator : AbstractValidator<TextToE1>
{
    public TextToE1Validator(bool isRequired = true)
    {
        When(_ => !string.IsNullOrWhiteSpace(_.Text), () => RuleFor(_ => _.Text).Must(_ => _?.Contains(';') == false));
        RuleFor(_ => _.Text).MaximumLength(30).WithName("TextToE1");

        if (isRequired)
        {
            RuleFor(_ => _.Text).NotEmpty().WithName("TextToE1");
        }
    }
}