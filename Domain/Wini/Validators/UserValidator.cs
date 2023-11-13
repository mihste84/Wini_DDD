namespace Domain.Wini.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator(bool isRequired = true, string name = "User ID")
    {
        RuleFor(_ => _.UserId).MaximumLength(8).WithName(name);

        if (isRequired)
            RuleFor(_ => _.UserId).NotEmpty().WithName(name);
    }
}