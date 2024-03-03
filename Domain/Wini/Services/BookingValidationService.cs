namespace Domain.Wini.Services;

public class BookingValidationService(
    IAuthorizationService authorizationService,
    IAuthorizerValidationService authorizerValidationService,
    IBookingPeriodValidationService bookingPeriodValidationService,
    IAccountingValidationService accountingValidationService
    ) : IBookingValidationService
{
    public async Task<BookingValidationResultModel> ValidateAsync(Booking booking, IEnumerable<Company> companies)
    {
        var tasks = new[] {
            ValidateValuesAsync(booking, companies),
            ValidateBookingPeriodsAsync(booking),
            ValidateAccountingValuesAsync(booking),
            ValidateAuthorizersAsync(booking)
        };

        foreach (var task in tasks)
        {
            var result = await task;
            if (!result.IsValid)
            {
                return result;
            }
        }

        return new();
    }

    private async Task<BookingValidationResultModel> ValidateAuthorizersAsync(Booking booking)
    {
        if (!authorizationService.IsBookingAuthorizationNeeded())
        {
            return new BookingValidationResultModel();
        }

        try
        {
            var (IsValid, Errors) = await authorizerValidationService.CanAuthorizeBookingRowsAsync(booking.Rows);
            return new BookingValidationResultModel
            {
                Errors = Errors,
                IsValid = IsValid,
                Message = !IsValid ? "One or more authorizers are not valid in MOA." : default
            };
        }
        catch (Exception ex)
        {
            throw new AuthorizerValidationException("An error occurred when validating authorizers with MOA.", ex);
        }
    }

    private static Task<BookingValidationResultModel> ValidateValuesAsync(Booking booking, IEnumerable<Company> companies)
    {
        var (IsValid, Errors) = booking.ValidateValues(companies);
        var res = new BookingValidationResultModel
        {
            Errors = Errors,
            IsValid = IsValid,
            Message = !IsValid ? "Booking failed to validate. One or more fields did not pass the validation rules." : default
        };
        return Task.FromResult(res);
    }

    private async Task<BookingValidationResultModel> ValidateBookingPeriodsAsync(Booking booking)
    {
        var (IsValid, Errors) = await bookingPeriodValidationService.ValidateAsync(booking);
        return new BookingValidationResultModel
        {
            Errors = Errors,
            IsValid = IsValid,
            Message = !IsValid
                ? $"Booking failed to validate. Accounting date '{booking.Header.BookingDate.Date: yyyy-MM-dd}' is not open for manual bookings."
                : default
        };
    }

    private async Task<BookingValidationResultModel> ValidateAccountingValuesAsync(Booking booking)
    {
        var inputModel = booking.Rows
            .Where(_ => _.CanRowBeAuthorized())
            .Select(_ => new AccountingValidationInputModel
                {
                    Account = _.Account.Value,
                    BookingRow = _.RowNumber,
                    BusinessUnit = _.BusinessUnit.ToString(),
                    CostObject1 = _.CostObject1.Value,
                    CostObject1Type = _.CostObject1.Type,
                    CostObject2 = _.CostObject2.Value,
                    CostObject2Type = _.CostObject2.Type,
                    CostObject3 = _.CostObject3.Value,
                    CostObject3Type = _.CostObject3.Type,
                    CostObject4 = _.CostObject4.Value,
                    CostObject4Type = _.CostObject4.Type,
                    Currency = _.Money.Currency.CurrencyCode.Code,
                    Subledger = _.Subledger.Value,
                    Subsidiary = _.Account.Subsidiary
                });

        try
        {
            var (IsValid, Errors) = await accountingValidationService.ValidateAsync(inputModel);
            return new BookingValidationResultModel
            {
                Errors = Errors,
                IsValid = IsValid,
                Message = !IsValid ? "One or several rows contain accounting errors." : default
            };
        }
        catch (Exception ex)
        {
            throw new AccountingValidationException(
                "An error occurred when validating accounting data with Enterprise One.", ex
            );
        }
    }
}