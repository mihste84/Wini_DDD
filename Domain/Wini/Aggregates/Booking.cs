namespace Domain.Wini.Aggregates;

public class Booking
{
    public readonly IdValue<int>? BookingId;
    public readonly Commissioner Commissioner;
    public BookingStatus BookingStatus { get; }
    public BookingHeader Header { get; private set; }
    public List<BookingRow> Rows { get; } = new();
    public List<Comment> Comments { get; } = new();
    public List<RecipientMessage> Messages { get; } = new();
    public List<Attachment> Attachments { get; } = new();
    public readonly DateTime Created;

    public Booking(
        IdValue<int>? id,
        Commissioner commissioner
    )
    {
        BookingId = id;
        BookingStatus = new BookingStatus(WiniStatus.Saved, DateTime.UtcNow);
        Commissioner = commissioner;
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
    }

    public void EditBookingHeader(BookingHeaderModel model)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot edit booking. Booking can only be edited when status is 'Saved'.");

        Header = new BookingHeader(
            model.BookingDate,
            model.TextToE1,
            model.IsReversed,
            model.LedgerType
        );
    }

    public void AddNewRow(BookingRowModel row)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot add new row. Rows can only be added when Booking status is 'Saved'.");

        if (Rows.Any(_ => _.RowNumber == row.RowNumber))
            throw new DomainLogicException($"Cannot add new row. Row number {row.RowNumber} already exists.");

        var newRow = MapBookingRowModelToValue(row);

        Rows.Add(newRow);
    }

    public void EditRow(BookingRowModel row)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot edit row. Rows can only be added when Booking status is 'Saved'.");

        var existingRowIndex = Rows.FindIndex(_ => _.RowNumber == row.RowNumber);

        if (existingRowIndex == -1)
            throw new NotFoundException($"Cannot edit row. Existing row with number {row.RowNumber} could not be found.");

        Rows[existingRowIndex] = MapBookingRowModelToValue(row);
    }

    public void DeleteRow(int rowNumber)
    {
        if (BookingStatus.Status != WiniStatus.Saved)
            throw new DomainLogicException("Cannot delete row. Rows can only be deleted when Booking status is 'Saved'.");

        var rowToDelete = Rows.SingleOrDefault(_ => _.RowNumber == rowNumber)
            ?? throw new NotFoundException($"Cannot delete row. Existing row with number {rowNumber} could not be found.");

        Rows.Remove(rowToDelete);
    }

    public async Task SetToBeAuthorizedStatus(IBookingValidationService validationService)
    {
        await validationService.ValidateAsync(this);

        BookingStatus.CanChangeStatusToBeAuthorized();

        BookingStatus.TryChangeStatus(WiniStatus.ToBeAuthorized);

        // Add event
    }

    public void SetCancelledStatus(IAuthenticationService authenticationService)
    {
        BookingStatus.CanChangeStatusToCancelled();

        var userId = authenticationService.GetUserId();
        if (userId != Commissioner.UserId)
            throw new DomainLogicException(nameof(userId), userId, $"Only commissioners can change status to {WiniStatus.Cancelled}.");

        BookingStatus.TryChangeStatus(WiniStatus.Cancelled);
        // Add event
    }

    public void SetSendErrorStatus(IAuthorizationService authorizationService)
    {
        BookingStatus.CanChangeStatusToSendError();

        if (!authorizationService.IsAdmin())
            throw new DomainLogicException($"Only admins can change status to {WiniStatus.SendError}.");

        RemoveAuthorizationAllRows();
        BookingStatus.TryChangeStatus(WiniStatus.SendError);

        // Add event
    }

    public void SetNotAuthorizedOnTimeStatus(IAuthorizationService authorizationService, DateTime now)
    {
        BookingStatus.CanChangeStatusToNotAuthorizedOnTime();

        if (!authorizationService.IsAdmin())
            throw new DomainLogicException($"Only admins can change status to {WiniStatus.NotAuthorizedOnTime}.");

        if (!HaveThreeDaysPassed(now))
            throw new DomainLogicException(nameof(now), now.ToString("yyyy-MM-dd HH:mm:ss"), $"Status cannot be changed to {WiniStatus.NotAuthorizedOnTime}. 72 hours have not passed yet.");

        BookingStatus.TryChangeStatus(WiniStatus.NotAuthorizedOnTime);
        BookingStatus.TryChangeStatus(WiniStatus.Saved); // Change directly to Saved. NotAutorizedOnTime is logged in history.

        // Add event
    }

    public void SetSavedStatus(IAuthenticationService authenticationService, IAuthorizationService authorizationService)
    {
        BookingStatus.CanChangeStatusToSaved();

        var userId = authenticationService.GetUserId();
        if (!(userId == Commissioner.UserId || Rows.Any(_ => _.Authorizer.UserId == userId) || authorizationService.IsAdmin()))
            throw new DomainLogicException($"Only admins, commissioners or authorizers can change status to {WiniStatus.Saved}.");

        RemoveAuthorizationAllRows();

        BookingStatus.TryChangeStatus(WiniStatus.Saved);
        // Add event
    }

    public async Task SetToBeSentStatus(
        IAuthenticationService authenticationService,
        IAuthorizationService authorizationService,
        IBookingValidationService validationService)
    {
        await validationService.ValidateAsync(this);

        BookingStatus.CanChangeStatusToBeSent();

        if (authorizationService.IsBookingAuthorizationNeeded())
        {
            var userId = authenticationService.GetUserId();
            AuthorizeRowsForUser(userId);
            if (Rows.Any(_ => _.Authorizer.HasAuthorized))
            {
                // More rows left to be authorized by other users. Just save status to history.
                BookingStatus.SaveStatusHistory();
                return;
            }
        }

        BookingStatus.TryChangeStatus(WiniStatus.ToBeSent);
        // Add event
    }

    public bool AreRowsInSequence()
    {
        var rowNumbers = Rows.Select(_ => _.RowNumber.GetValueOrDefault()).ToArray();
        return rowNumbers.SequenceEqual(Enumerable.Range(1, rowNumbers.Length));
    }

    public bool AreAllCompaniesSame()
    {
        var firstCompanyValue = Rows.FirstOrDefault()?.BusinessUnit.CompanyCode.Code;
        return firstCompanyValue.HasValue && !Rows.All(_ => _.BusinessUnit.CompanyCode.Code != firstCompanyValue);
    }

    public IEnumerable<string?> GetAllAuthorizers() => Rows.Select(_ => _.Authorizer.UserId);

    public (bool IsValid, IEnumerable<ValidationError>? Errors) ValidateValues(IEnumerable<Company> companies)
    {
        var validator = new BookingValidator(companies);
        var res = validator.Validate(this);

        return (res.IsValid, res.Errors?.Select(_ => new ValidationError(_)));
    }

    private bool HaveThreeDaysPassed(DateTime now)
    {
        var futureDate = BookingStatus.Updated.AddDays(3);
        var time = futureDate - now;
        return time.TotalHours < 0;
    }

    private void AuthorizeRowsForUser(string userId)
    {
        if (!Rows.Any(_ => _.CanRowBeAuthorizedByUser(userId)))
            throw new DomainLogicException(nameof(userId), userId, "There are no rows to authorize for user.");

        AuthorizeRowsByUserId(userId);
    }

    private void AuthorizeRowsByUserId(string userId)
    {
        for (var i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].CanRowBeAuthorizedByUser(userId))
                Rows[i] = Rows[i].ChangeRowAuthorization(true);
        }
    }

    private void RemoveAuthorizationAllRows()
    {
        for (var i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].Authorizer.HasAuthorized)
                Rows[i] = Rows[i].ChangeRowAuthorization(false);
        }
    }

    private static BookingRow MapBookingRowModelToValue(BookingRowModel row)
    => new(
        row.RowNumber,
        new BusinessUnit(row.BusinessUnit),
        new Account(row.Account, row.Subsidiary),
        new Subledger(row.Subledger, row.SubledgerType),
        new CostObject(1, row.CostObject1, row.CostObjectType1),
        new CostObject(2, row.CostObject2, row.CostObjectType2),
        new CostObject(3, row.CostObject3, row.CostObjectType3),
        new CostObject(4, row.CostObject4, row.CostObjectType4),
        new Remark(row.Remark),
        new Authorizer(row.Authorizer, false),
        new Money(row.Amount, row.CurrencyCode, row.CurrencyRate)
    );
}