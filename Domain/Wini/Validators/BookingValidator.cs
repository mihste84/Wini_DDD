namespace Domain.Wini.Validators;
public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator(IEnumerable<Company> companies)
    {
        RuleFor(_ => _.BookingId).SetValidator(new BookingIdValidator());
        RuleFor(_ => _.Commissioner).SetValidator(new UserValidator(true, "Commissioner"));
        RuleFor(_ => _.Header).SetValidator(new BookingHeaderValidator());
        RuleFor(_ => _.Rows).NotEmpty().WithMessage("Booking must contain rows.");

        When(_ => _.Rows.Any(), () => RuleFor(_ => _).Custom((booking, ctx)
            => ValidateAllRows(booking, companies, ctx))
        );
    }

    private static void ValidateAllRows(Booking booking, IEnumerable<Company> companies, ValidationContext<Booking> ctx)
    {
        if (!booking.AreRowsInSequence())
            ctx.AddFailure("Row Numbers", "Row numbers are not in sequence.");

        if (!booking.AreAllCompaniesSame())
            ctx.AddFailure("Company", "All rows dont contain same company code. Only one company code can be used for each booking.");

        var validator = new BookingRowValidator(booking, companies);
        foreach (var row in booking.Rows)
            ValidateRow(row, validator, ctx);
    }

    private static void ValidateRow(BookingRow row, BookingRowValidator validator, ValidationContext<Booking> ctx)
    {
        var result = validator.Validate(row);
        if (result.IsValid)
            return;

        foreach (var error in result.Errors)
            ctx.AddFailure($"{error.PropertyName} (Row: {row.RowNumber})", error.ErrorMessage);
    }
}