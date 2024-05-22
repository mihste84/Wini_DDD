namespace Domain.Common.Validators;
public class CountryValidator : AbstractValidator<Country>
{
    public CountryValidator()
    {
        RuleFor(_ => _.Code).NotEmpty().MaximumLength(2).WithName("Country Code");
        RuleFor(_ => _.Name).MaximumLength(50).WithName("Country");
    }
}