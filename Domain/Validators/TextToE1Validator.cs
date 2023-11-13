namespace Domain.Validators;
public class TextToE1Validator : AbstractValidator<TextToE1>
{
    public TextToE1Validator(bool isRequired = true)
    {
        RuleFor(_ => _.Text).MaximumLength(30).WithName("TextToE1");

        if (isRequired)
        {
            RuleFor(_ => _.Text).NotEmpty().WithName("TextToE1");
        }
    }
}