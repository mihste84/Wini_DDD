namespace Domain.Wini.Validators;
public class BusinessUnitValidator : AbstractValidator<BusinessUnit>
{
    public BusinessUnitValidator()
    {
        // RuleFor(_ => _.Costcenter).SetValidator(new CostcenterValidator(isRequired));
        // RuleFor(_ => _.Product).SetValidator(new ProductValidator());
        // RuleFor(_ => _.CompanyCode).SetValidator(new CompanyCodeValidator(isRequired));

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