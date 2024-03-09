namespace Domain.Wini.Aggregates;

public partial class Booking
{
    public bool AreRowsInSequence()
    {
        var rowNumbers = Rows.Select(_ => _.RowNumber).ToArray();

        for (int i = 0; i < rowNumbers.Length - 1; i++)
        {
            if (rowNumbers[i] > rowNumbers[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    public void EditBookingHeader(BookingHeaderModel model, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        Header = new BookingHeader(
            model.BookingDate,
            model.TextToE1,
            model.IsReversed,
            model.LedgerType
        );

        BookingStatus.SaveStatusHistory(); // Save history on update
    }

    public void AddNewRow(BookingRowModel row, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        AddNewRowToList(row);
    }

    public void AddMultipleRows(BookingRowModel[] rows, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        foreach (var row in rows)
        {
            AddNewRowToList(row);
        }

        VerifyIfRowsAreInSequence();
    }

    public void EditRow(BookingRowModel row, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        if (!TryGetExistingRowIndex(row.RowNumber, out var existingRowIndex))
        {
            throw new NotFoundException($"Cannot update row. Existing row with number {row.RowNumber} could not be found.");
        }

        ReplaceRowByIndex(row, existingRowIndex);
    }

    public void UpsertMultipleRows(BookingRowModel[] rows, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        foreach (var row in rows)
        {
            UpsertRow(row);
        }

        VerifyIfRowsAreInSequence();
    }

    public void DeleteRow(int rowNumber, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        DeleteRowFromList(rowNumber);
    }

    public void DeleteMultipleRows(int[] rowNumbers, IAuthenticationService authenticationService)
    {
        VerifyIfBookingIsEditable(authenticationService.GetUserId());

        foreach (var rowNumber in rowNumbers)
        {
            DeleteRowFromList(rowNumber);
        }

        VerifyIfRowsAreInSequence();
    }

    private void AddDeleteRowEvent(int rowNumber, int bookingId)
        => DomainEvents.Add(new WiniBookingRowDeleteEvent(rowNumber, bookingId));

    private void AddRowActionEvent(BookingRowAction action, BookingRow row, int? bookingId)
        => DomainEvents.Add(new WiniBookingRowActionEvent(action, row, bookingId));

    private void AuthorizeRowsForUser(string userId, int bookingId)
    {
        if (!Rows.Exists(_ => _.CanRowBeAuthorizedByUser(userId)))
        {
            throw new DomainLogicException(nameof(userId), userId, "There are no rows to authorize for user.");
        }

        AuthorizeRowsByUserId(userId, bookingId);
    }

    private void AuthorizeRowsByUserId(string userId, int bookingId)
    {
        for (var i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].CanRowBeAuthorizedByUser(userId))
            {
                var row = Rows[i].ChangeRowAuthorization(true);
                AddRowActionEvent(BookingRowAction.Authorized, row, bookingId);
                Rows[i] = row;
            }
        }
    }

    private void RemoveAuthorizationAllRows(int bookingId)
    {
        for (var i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].Authorizer.HasAuthorized)
            {
                var row = Rows[i].ChangeRowAuthorization(false);
                AddRowActionEvent(BookingRowAction.RemoveAuthorization, row, bookingId);
                Rows[i] = row;
            }
        }
    }

    private void ReplaceRowByIndex(BookingRowModel row, int existingRowIndex)
    {
        var changedRow = MapBookingRowModelToValue(row);
        Rows[existingRowIndex] = changedRow;
        AddRowActionEvent(BookingRowAction.Updated, changedRow, BookingId!.Value);
    }

    private bool TryGetExistingRowIndex(int rowNumber, out int index)
    {
        index = Rows.FindIndex(_ => _.RowNumber == rowNumber);
        return index != -1;
    }

    private void UpsertRow(BookingRowModel row)
    {
        if (TryGetExistingRowIndex(row.RowNumber, out var existingRowIndex))
        {
            ReplaceRowByIndex(row, existingRowIndex);
        }
        else
        {
            AddNewRowToList(row);
        }
    }

    private void DeleteRowFromList(int rowNumber)
    {
        var rowToDelete = Rows.SingleOrDefault(_ => _.RowNumber == rowNumber)
            ?? throw new NotFoundException($"Cannot delete row. Existing row with number {rowNumber} could not be found.");

        Rows.Remove(rowToDelete);
        AddDeleteRowEvent(rowNumber, BookingId!.Value);
    }

    private void VerifyIfRowsAreInSequence()
    {
        if (AreRowsInSequence())
        {
            return;
        }

        throw new DomainLogicException("Cannot update rows. Row numbers not in sequence.");
    }

    private void VerifyIfBookingIsEditable(string userId)
    {
        if (userId != Commissioner.UserId)
        {
            throw new DomainLogicException("Only commissioner make changes to booking.");
        }

        if (BookingStatus.Status == WiniStatus.Saved)
        {
            return;
        }

        throw new DomainLogicException("Changes can only be made when Booking status is 'Saved'.");
    }

    private void AddNewRowToList(BookingRowModel row)
    {
        if (Rows.Exists(_ => _.RowNumber == row.RowNumber))
        {
            throw new DomainLogicException($"Cannot add new row. Row number {row.RowNumber} already exists.");
        }

        var newRow = MapBookingRowModelToValue(row);

        Rows.Add(newRow);
        AddRowActionEvent(BookingRowAction.Added, newRow, BookingId?.Value);
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
        new Money(row.Amount, row.CurrencyCode, row.ExchangeRate)
    );
}