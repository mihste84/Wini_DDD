namespace Domain.Wini.Validators;
public class BookingRowValidator : AbstractValidator<BookingRow>
{
    public BookingRowValidator(Booking booking, IEnumerable<Company> companies)
    {
        RuleFor(_ => _.Account).SetValidator(new AccountValidator());
        RuleFor(_ => _.BusinessUnit).SetValidator(new BusinessUnitValidator());
        RuleFor(_ => _.Subledger).SetValidator(new SubledgerValidator());
        RuleFor(_ => _.CostObject1).SetValidator(new CostObjectValidator(1));
        RuleFor(_ => _.CostObject2).SetValidator(new CostObjectValidator(2));
        RuleFor(_ => _.CostObject3).SetValidator(new CostObjectValidator(3));
        RuleFor(_ => _.CostObject4).SetValidator(new CostObjectValidator(4));
        RuleFor(_ => _.Remark).SetValidator(new RemarkValidator());
        RuleFor(_ => _.Money).SetValidator(new MoneyValidator());
        When(_ => _.Money.IsDebitRow(), () => RuleFor(_ => _.Authorizer).SetValidator(new AuthorizerValidator()))
            .Otherwise(() =>
                RuleFor(_ => _.Authorizer.UserId)
                    .Empty()
                    .WithName("Authorizer")
                    .WithMessage("Authorizer cannot be set on credit rows.")
            );

        RuleFor(_ => _).Custom((row, ctx) =>
        {
            if (!string.IsNullOrWhiteSpace(row.BusinessUnit.ToString()) && !row.IsCompanyCodeAllowed(companies))
            {
                ctx.AddFailure("Company Code", $"Company code '{row.BusinessUnit.CompanyCode.Code}' cannot be used.");
                return;
            }

            var isBaseCurrencyUsed = row.IsBaseCurrencyUsed(companies);
            if (
                !string.IsNullOrWhiteSpace(row.Money.Currency.CurrencyCode.Code) &&
                booking.Header.LedgerType.Type == Ledgers.GP &&
                !isBaseCurrencyUsed
            )
            {
                ctx.AddFailure("GP ledger", $"Cannot use GP ledger as currency '{row.Money.Currency.CurrencyCode.Code}' is not base currency for company '{row.BusinessUnit.CompanyCode.Code}'.");
                return;
            }

            if (
                !string.IsNullOrWhiteSpace(row.Money.Currency.CurrencyCode.Code) &&
                booking.Header.LedgerType.Type == Ledgers.AA &&
                !row.Money.IsCurrencyAndExchangeRateSet() &&
                !isBaseCurrencyUsed
            )
            {
                ctx.AddFailure("Currency", $"Exchange rate with currency '{row.Money.Currency.CurrencyCode.Code}' must be set when foreign currency is used.");
            }
        });
    }
}