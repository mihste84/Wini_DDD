namespace Domain.Validators;
public class BookingRowValidator : AbstractValidator<BookingRow>
{
    public BookingRowValidator()
    {
        RuleFor(_ => _.BookingId).SetValidator(new BookingIdValidator());
        RuleFor(_ => _.RowNumber).SetValidator(new BookingRowNumberValidator());
        RuleFor(_ => _.Account).SetValidator(new AccountValidator(true));
        RuleFor(_ => _.BusinessUnit).SetValidator(new BusinessUnitValidator());
        RuleFor(_ => _.Subledger).SetValidator(new SubledgerValidator());
        RuleFor(_ => _.CostObject1).SetValidator(new CostObjectValidator());
        RuleFor(_ => _.CostObject2).SetValidator(new CostObjectValidator());
        RuleFor(_ => _.CostObject3).SetValidator(new CostObjectValidator());
        RuleFor(_ => _.CostObject4).SetValidator(new CostObjectValidator());
        RuleFor(_ => _.Remark).SetValidator(new RemarkValidator());
        RuleFor(_ => _.Authorizer).SetValidator(new AuthorizerValidator());
        RuleFor(_ => _.Amount).SetValidator(new MoneyValidator(true));
    }
}