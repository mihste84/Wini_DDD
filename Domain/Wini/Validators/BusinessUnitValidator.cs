namespace Domain.Wini.Validators;
public class BusinessUnitValidator : AbstractValidator<BusinessUnit>
{
    public BusinessUnitValidator()
    {
        RuleFor(_ => _).Custom((bu, ctx) =>
        {
            if (!(bu.ToString()?.Length > 12))
            {
                return;
            }

            ctx.AddFailure("Business Unit", "BusinessUnit max length is 12 characters");
        });
    }
}