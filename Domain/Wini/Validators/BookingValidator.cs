namespace Domain.Wini.Validators;
public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator(IEnumerable<Company> companies)
    {
        RuleFor(_ => _.BookingId).SetValidator(new BookingIdValidator());
        RuleFor(_ => _.Commissioner).SetValidator(new UserValidator(true, "Commissioner"));
        RuleFor(_ => _.Header).SetValidator(new BookingHeaderValidator());
        RuleFor(_ => _.Rows).NotEmpty().WithMessage("Booking must contain rows.");

        When(
            _ => _.Rows.Count > 0,
            () => RuleFor(_ => _).Custom((booking, ctx)
                => ValidateAllRows(booking, companies, ctx))
        );
    }

    private static void ValidateAllRows(Booking booking, IEnumerable<Company> companies, ValidationContext<Booking> ctx)
    {
        if (!booking.AreRowsInSequence())
        {
            ctx.AddFailure("Row Numbers", "Row numbers are not in sequence.");
        }

        if (!booking.AreAllCompaniesSame())
        {
            ctx.AddFailure("Company", "All rows dont contain same company code. Only one company code can be used for each booking.");
            return;
        }

        if (!booking.TryValidateExchangeRateDifferencesByCurrency(out var rateDifferences))
        {
            foreach (var item in rateDifferences)
            {
                ctx.AddFailure("Exchange Rate", $"Exchange rates '{string.Join(", ", item.ExchangeRates)}' do not balance with currency code '{item.Currency}'.");
            }

            return;
        }

        if (!booking.TryValidateBalanceDifferencesByCurrency(out var balanceDifferences))
        {
            foreach (var item in balanceDifferences)
            {
                ctx.AddFailure("Debit & Credit", $"Debit and credit must be equal when using currency code '{item.Currency}'. Balance = {item.Balance}");
            }

            return;
        }

        var validator = new BookingRowValidator(booking, companies);
        foreach (var row in booking.Rows)
        {
            ValidateRow(row, validator, ctx);
        }
    }

    private static void ValidateRow(BookingRow row, BookingRowValidator validator, ValidationContext<Booking> ctx)
    {
        var result = validator.Validate(row);
        if (result.IsValid)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            ctx.AddFailure($"#{row.RowNumber}", error.ErrorMessage);
        }
    }
}