namespace Domain.Wini.Validators;
public class BookingRowValidator : AbstractValidator<BookingRow>
{
    public BookingRowValidator()
    {
        RuleFor(_ => _.Account).SetValidator(new AccountValidator());
        RuleFor(_ => _.BusinessUnit).SetValidator(new BusinessUnitValidator());
        RuleFor(_ => _.Subledger).SetValidator(new SubledgerValidator());
        RuleFor(_ => _.CostObject1).SetValidator(new CostObjectValidator(1));
        RuleFor(_ => _.CostObject2).SetValidator(new CostObjectValidator(2));
        RuleFor(_ => _.CostObject3).SetValidator(new CostObjectValidator(3));
        RuleFor(_ => _.CostObject4).SetValidator(new CostObjectValidator(4));
        RuleFor(_ => _.Remark).SetValidator(new RemarkValidator());
        RuleFor(_ => _.Authorizer).SetValidator(new AuthorizerValidator());
        RuleFor(_ => _.Amount).SetValidator(new MoneyValidator());
    }
}