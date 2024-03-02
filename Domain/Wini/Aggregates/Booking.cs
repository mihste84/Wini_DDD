namespace Domain.Wini.Aggregates;

public partial class Booking
{
    public readonly IdValue<int>? BookingId;
    public readonly Commissioner Commissioner;
    public BookingStatus BookingStatus { get; }
    public BookingHeader Header { get; private set; }
    public List<BookingRow> Rows { get; } = new();
    public List<Comment> Comments { get; } = new();
    public List<RecipientMessage> Messages { get; } = new();
    public List<Attachment> Attachments { get; } = new();
    public List<BaseDomainEvent> DomainEvents { get; } = new();
    public readonly DateTime Created;

    public Booking(
        IdValue<int>? id,
        Commissioner commissioner
    )
    {
        BookingId = id;
        Commissioner = commissioner;
        BookingStatus = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow, commissioner.UserId!);
        Header = new BookingHeader();
        Created = DateTime.UtcNow;
    }

    public Booking(
        IdValue<int>? id,
        BookingStatus status,
        Commissioner commissioner,
        BookingDate bookingDate,
        TextToE1 textToE1,
        bool isReversed,
        LedgerType ledgerType
    )
    {
        BookingId = id;
        BookingStatus = status;
        Commissioner = commissioner;
        Header = new BookingHeader(bookingDate, textToE1, isReversed, ledgerType);
        Created = DateTime.UtcNow;
    }

    public Booking(
        IdValue<int>? id,
        BookingStatus status,
        Commissioner commissioner,
        BookingDate bookingDate,
        TextToE1 textToE1,
        bool isReversed,
        LedgerType ledgerType,
        List<BookingRow> rows,
        List<Comment> comments,
        List<RecipientMessage> messages,
        List<Attachment> attachments,
        DateTime created
    )
    {
        BookingId = id;
        BookingStatus = status;
        Commissioner = commissioner;
        Header = new BookingHeader(bookingDate, textToE1, isReversed, ledgerType);
        Rows = rows;
        Comments = comments;
        Messages = messages;
        Attachments = attachments;
        Created = created;

        if (Rows.Count > 0 && !AreRowsInSequence())
        {
            var errors = new[] {
                new ValidationError { Message = "Row numbers are not in sequence.", PropertyName = "Row Numbers" }
            };

            throw new DomainValidationException($"Failed to validate booking {BookingId?.Value}.", errors);
        }
    }

    public (bool CanDelete, string? Reason) CanDeleteBooking(IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        if (BookingStatus.Status == WiniStatus.Sent)
            return (false, "AlreadySent");

        return (authenticationService.GetUserId() == Commissioner.UserId || authorizationService.IsAdmin())
        ? (true, default)
        : (false, "Forbidden");
    }

    public bool AreAllCompaniesSame()
    {
        var firstCompanyValue = Rows.FirstOrDefault()?.BusinessUnit.CompanyCode.Code;
        return Rows.All(_ => _.BusinessUnit.CompanyCode.Code == firstCompanyValue);
    }

    public bool TryValidateExchangeRateDifferencesByCurrency(out IEnumerable<(string? Currency, decimal?[] ExchangeRates)> differences)
    {
        var ratesByCurrency = Rows
            .GroupBy(_ => _.Money.Currency.CurrencyCode.Code)
            .Select(_ => new { Currency = _.Key, ExchangeRates = _.Select(x => x.Money.Currency.ExchangeRate).Distinct().ToArray() });

        differences = ratesByCurrency
            .Where(_ => !_.ExchangeRates.All(x => x == _.ExchangeRates[0]))
            .Select(_ => (_.Currency, _.ExchangeRates));

        return !differences.Any();
    }

    public bool TryValidateBalanceDifferencesByCurrency(out IEnumerable<(string? Currency, decimal? Balance)> differences)
    {
        var amountsPerCurrency = Rows
            .GroupBy(_ => _.Money.Currency.CurrencyCode.Code)
            .Select(_ => new { Currency = _.Key, Balance = _.Sum(x => x.Money.Amount) });

        differences = amountsPerCurrency
            .Where(_ => _.Balance != 0)
            .Select(_ => (_.Currency, _.Balance));

        return !differences.Any();
    }

    public (bool IsValid, IEnumerable<ValidationError>? Errors) ValidateValues(IEnumerable<Company> companies)
    {
        var validator = new BookingValidator(companies);
        var res = validator.Validate(this);

        return (res.IsValid, res.Errors?.Select(_ => new ValidationError(_)));
    }
}