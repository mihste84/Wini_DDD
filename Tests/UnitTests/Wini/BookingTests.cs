namespace Tests.UnitTests.Wini;

public class BookingTests
{
    private readonly IEnumerable<Company> _companies = CommonTestValues.GetCompanies();

    [Fact]
    public void Create_New_Booking_With_No_Data()
    {
        var booking = CommonTestValues.GetNewEmptyBooking();

        var today = DateTime.UtcNow.Date;
        Assert.Equal(default, booking.BookingId);
        Assert.Equal(today, booking.Created.Date);
        Assert.Equal(today, booking.Header.BookingDate.Date);
        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        Assert.Equal(today, booking.BookingStatus.Updated.Date);
        Assert.Equal("MIHSTE", booking.Commissioner.UserId);
        Assert.False(booking.Header.IsReversed);
        Assert.Equal(Ledgers.AA, booking.Header.LedgerType.Type);
        Assert.Equal(default, booking.Header.TextToE1.Text);
        Assert.Empty(booking.Rows);
        Assert.Empty(booking.Attachments);
        Assert.Empty(booking.Comments);
        Assert.Empty(booking.Messages);
    }

    [Fact]
    public void Create_New_Booking_With_Data()
    {
        var booking = CommonTestValues.GetBooking();

        Assert.Equal(1, booking.BookingId?.Value);
        Assert.Equal(new DateTime(2023, 3, 20, 23, 0, 0), booking.Created);
        Assert.Equal(new DateTime(2023, 3, 22), booking.Header.BookingDate.Date);
        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        Assert.Equal(new DateTime(2023, 3, 20, 23, 0, 0), booking.BookingStatus.Updated);
        Assert.Equal("XMIHST", booking.Commissioner.UserId);
        Assert.True(booking.Header.IsReversed);
        Assert.Equal(Ledgers.GP, booking.Header.LedgerType.Type);
        Assert.Equal("Test", booking.Header.TextToE1.Text);
        Assert.Empty(booking.Rows);
        Assert.Empty(booking.Attachments);
        Assert.Empty(booking.Comments);
        Assert.Empty(booking.Messages);
    }

    [Fact]
    public void Edit_Header_To_Existing_Booking()
    {
        var bookingDate = new DateTime(2023, 1, 1);
        var booking = CommonTestValues.GetBooking();
        var header = new BookingHeaderModel(
            bookingDate,
            "EDIT TEST",
            true,
            Ledgers.GP
        );
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.EditBookingHeader(header, authenticationService.Object);

        Assert.True(booking.Header.IsReversed);
        Assert.Equal(Ledgers.GP, booking.Header.LedgerType.Type);
        Assert.Equal("EDIT TEST", booking.Header.TextToE1.Text);
        Assert.Equal(bookingDate, booking.Header.BookingDate.Date);
    }

    [Fact]
    public void Add_New_Row_To_Empty_Booking()
    {
        var booking = CommonTestValues.GetNewEmptyBooking();
        var newRow = GetRowModel();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.AddNewRow(newRow, authenticationService.Object);

        Assert.Single(booking.Rows);
        var row = booking.Rows.Single();
        Assert.Equal(1, row.RowNumber);
        Assert.Equal("12345", row.Account.Value);
        Assert.Equal("7894", row.Account.Subsidiary);
        Assert.Equal(100, row.Money.Amount);
        Assert.Equal("EUR", row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(10, row.Money.Currency.ExchangeRate);
        Assert.Equal("MIHSTE", row.Authorizer.UserId);
        Assert.False(row.Authorizer.HasAuthorized);
        Assert.Equal("100KKTOT8888", row.BusinessUnit.ToString());
        Assert.Equal(100, row.BusinessUnit.CompanyCode.Code);
        Assert.Equal("KKTOT", row.BusinessUnit.Costcenter.Code);
        Assert.Equal("8888", row.BusinessUnit.Product.Code);
        Assert.Equal(1, row.CostObject1.Number);
        Assert.Equal("CO1", row.CostObject1.Value);
        Assert.Equal("A", row.CostObject1.Type);
        Assert.Equal(2, row.CostObject2.Number);
        Assert.Equal("CO2", row.CostObject2.Value);
        Assert.Equal("B", row.CostObject2.Type);
        Assert.Equal(3, row.CostObject3.Number);
        Assert.Equal("CO3", row.CostObject3.Value);
        Assert.Equal("C", row.CostObject3.Type);
        Assert.Equal(4, row.CostObject4.Number);
        Assert.Equal("CO4", row.CostObject4.Value);
        Assert.Equal(default, row.CostObject4.Type);
        Assert.Equal("TEST", row.Remark.Text);
        Assert.Equal("990099", row.Subledger.Value);
        Assert.Equal("A", row.Subledger.Type);

        Assert.Single(booking.DomainEvents);
        var evt = booking.DomainEvents.FirstOrDefault() as WiniBookingRowActionEvent;
        Assert.NotNull(evt);
        Assert.Equal(BookingRowAction.Added, evt.Action);
        Assert.Equal(row, evt.Row);
        Assert.Null(evt.BookingId);
    }

    [Fact]
    public void Add_Multiple_New_Rows_To_Empty_Booking()
    {
        var booking = CommonTestValues.GetNewEmptyBooking();
        var rows = new[] {
            GetRowModel(1),
            GetRowModel(2)
        };
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.AddMultipleRows(rows, authenticationService.Object);

        Assert.Equal(2, booking.Rows.Count);
        Assert.All(booking.DomainEvents, _ => Assert.IsType<WiniBookingRowActionEvent>(_));
    }

    [Fact]
    public void Add_New_Row_To_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.AddNewRow(newRow, authenticationService.Object);

        Assert.Single(booking.Rows);
        var row = booking.Rows.Single();
        Assert.Equal(1, row.RowNumber);
        Assert.Equal("12345", row.Account.Value);
        Assert.Equal("7894", row.Account.Subsidiary);
        Assert.Equal(100, row.Money.Amount);
        Assert.Equal("EUR", row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(10, row.Money.Currency.ExchangeRate);
        Assert.Equal("MIHSTE", row.Authorizer.UserId);
        Assert.False(row.Authorizer.HasAuthorized);
        Assert.Equal("100KKTOT8888", row.BusinessUnit.ToString());
        Assert.Equal(100, row.BusinessUnit.CompanyCode.Code);
        Assert.Equal("KKTOT", row.BusinessUnit.Costcenter.Code);
        Assert.Equal("8888", row.BusinessUnit.Product.Code);
        Assert.Equal(1, row.CostObject1.Number);
        Assert.Equal("CO1", row.CostObject1.Value);
        Assert.Equal("A", row.CostObject1.Type);
        Assert.Equal(2, row.CostObject2.Number);
        Assert.Equal("CO2", row.CostObject2.Value);
        Assert.Equal("B", row.CostObject2.Type);
        Assert.Equal(3, row.CostObject3.Number);
        Assert.Equal("CO3", row.CostObject3.Value);
        Assert.Equal("C", row.CostObject3.Type);
        Assert.Equal(4, row.CostObject4.Number);
        Assert.Equal("CO4", row.CostObject4.Value);
        Assert.Equal(default, row.CostObject4.Type);
        Assert.Equal("TEST", row.Remark.Text);
        Assert.Equal("990099", row.Subledger.Value);
        Assert.Equal("A", row.Subledger.Type);
    }

    [Fact]
    public void Edit_Row_Of_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(10);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.AddNewRow(newRow, authenticationService.Object);
        var addedRow = booking.Rows.Single();

        var editRow = GetRowModel(10);
        editRow.Account = "98765";
        editRow.Subsidiary = "6543";
        editRow.Amount = 200;
        editRow.CurrencyCode = "SEK";
        editRow.ExchangeRate = -10;
        editRow.Authorizer = default;
        editRow.BusinessUnit = "100NN123";
        editRow.CostObject1 = "CX1";
        editRow.CostObjectType1 = "X";
        editRow.CostObject2 = default;
        editRow.CostObjectType2 = default;
        editRow.CostObject3 = default;
        editRow.CostObjectType3 = default;
        editRow.CostObject4 = default;
        editRow.CostObjectType4 = default;
        editRow.Remark = "TEST row";
        editRow.Subledger = default;
        editRow.SubledgerType = default;

        booking.EditRow(editRow, authenticationService.Object);

        Assert.Single(booking.Rows);
        var row = booking.Rows.Single();
        Assert.Equal(editRow.RowNumber, row.RowNumber);
        Assert.Equal(editRow.Account, row.Account.Value);
        Assert.Equal(editRow.Subsidiary, row.Account.Subsidiary);
        Assert.Equal(editRow.Amount, row.Money.Amount);
        Assert.Equal(editRow.CurrencyCode, row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(editRow.ExchangeRate, row.Money.Currency.ExchangeRate);
        Assert.Equal(editRow.Authorizer, row.Authorizer.UserId);
        Assert.False(row.Authorizer.HasAuthorized);
        Assert.Equal(editRow.BusinessUnit, row.BusinessUnit.ToString());
        Assert.Equal(100, row.BusinessUnit.CompanyCode.Code);
        Assert.Equal("NN123", row.BusinessUnit.Costcenter.Code);
        Assert.Equal(default, row.BusinessUnit.Product.Code);
        Assert.Equal(1, row.CostObject1.Number);
        Assert.Equal(editRow.CostObject1, row.CostObject1.Value);
        Assert.Equal(editRow.CostObjectType1, row.CostObject1.Type);
        Assert.Equal(2, row.CostObject2.Number);
        Assert.Equal(editRow.CostObject2, row.CostObject2.Value);
        Assert.Equal(editRow.CostObjectType2, row.CostObject2.Type);
        Assert.Equal(3, row.CostObject3.Number);
        Assert.Equal(editRow.CostObject3, row.CostObject3.Value);
        Assert.Equal(editRow.CostObjectType3, row.CostObject3.Type);
        Assert.Equal(4, row.CostObject4.Number);
        Assert.Equal(editRow.CostObject4, row.CostObject4.Value);
        Assert.Equal(editRow.CostObjectType4, row.CostObject4.Type);
        Assert.Equal(editRow.Remark, row.Remark.Text);
        Assert.Equal(editRow.Subledger, row.Subledger.Value);
        Assert.Equal(editRow.SubledgerType, row.Subledger.Type);

        Assert.Equal(2, booking.DomainEvents.Count);
        var evt1 = booking.DomainEvents[0] as WiniBookingRowActionEvent;
        Assert.NotNull(evt1);
        Assert.Equal(BookingRowAction.Added, evt1.Action);
        Assert.Equal(addedRow, evt1.Row);
        Assert.Equal(1, evt1.BookingId);
        var evt2 = booking.DomainEvents[1] as WiniBookingRowActionEvent;
        Assert.NotNull(evt2);
        Assert.Equal(BookingRowAction.Updated, evt2.Action);
        Assert.Equal(row, evt2.Row);
        Assert.Equal(1, evt2.BookingId);
    }

    [Fact]
    public void Upsert_Rows_Of_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(1);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        booking.AddNewRow(newRow, authenticationService.Object);

        var editRow = GetRowModel(1);
        editRow.Account = "12345";
        var rowsToUpsert = new[] {
            editRow,
            GetRowModel(2)
        };

        booking.UpsertMultipleRows(rowsToUpsert, authenticationService.Object);

        Assert.Equal(2, booking.Rows.Count);
        Assert.Contains(booking.Rows, _ => _.RowNumber == 1 && _.Account.Value == "12345");
        Assert.Contains(booking.Rows, _ => _.RowNumber == 2);
        Assert.Equal(3, booking.DomainEvents.Count);
        var evt1 = booking.DomainEvents[1] as WiniBookingRowActionEvent;
        Assert.NotNull(evt1);
        Assert.Equal(BookingRowAction.Updated, evt1.Action);
        Assert.Equal(booking.Rows[0], evt1.Row);
        Assert.Equal(1, evt1.BookingId);
        var evt2 = booking.DomainEvents[2] as WiniBookingRowActionEvent;
        Assert.NotNull(evt2);
        Assert.Equal(BookingRowAction.Added, evt2.Action);
        Assert.Equal(booking.Rows[1], evt2.Row);
        Assert.Equal(1, evt2.BookingId);
    }

    [Fact]
    public void Delete_Row_Of_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(10);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        booking.AddNewRow(newRow, authenticationService.Object);
        var row = booking.Rows[0];

        booking.DeleteRow(10, authenticationService.Object);

        Assert.Empty(booking.Rows);
        Assert.Equal(2, booking.DomainEvents.Count);
        var evt1 = booking.DomainEvents[0] as WiniBookingRowActionEvent;
        Assert.NotNull(evt1);
        Assert.Equal(BookingRowAction.Added, evt1.Action);
        Assert.Equal(row, evt1.Row);
        Assert.Equal(1, evt1.BookingId);
        var evt2 = booking.DomainEvents[1] as WiniBookingRowDeleteEvent;
        Assert.NotNull(evt2);
        Assert.Equal(10, evt2.RowNumber);
        Assert.Equal(1, evt2.BookingId);
    }

    [Fact]
    public void Delete_Multiple_Rows_Of_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(1);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        var rowsToInsert = new[] {
            GetRowModel(1),
            GetRowModel(2),
            GetRowModel(3)
        };
        booking.AddMultipleRows(rowsToInsert, authenticationService.Object);

        var rowsToDelete = new[] { 1, 3 };

        booking.DeleteMultipleRows(rowsToDelete, authenticationService.Object);

        Assert.Single(booking.Rows);
        Assert.Contains(booking.Rows, _ => _.RowNumber == 2);
        Assert.Contains(booking.DomainEvents, _ => _ is WiniBookingRowDeleteEvent);
    }

    [Fact]
    public void Fail_To_Add_New_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRow(newRow, authenticationService.Object));

        Assert.Equal("Changes can only be made when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Add_New_Row_Duplicate_Row_Number()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Saved);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);

        var newRow = GetRowModel(1);
        booking.AddNewRow(newRow, authenticationService.Object);
        newRow = GetRowModel(1);

        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRow(newRow, authenticationService.Object));

        Assert.Equal("Cannot add new row. Row number 1 already exists.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        var newRow = GetRowModel();

        var ex = Assert.Throws<DomainLogicException>(() => booking.EditRow(newRow, authenticationService.Object));

        Assert.Equal("Changes can only be made when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Row_Number()
    {
        var booking = CommonTestValues.GetBooking();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        var newRow = GetRowModel(10);
        var editRow = GetRowModel(11);
        var ex = Assert.Throws<NotFoundException>(() => booking.EditRow(editRow, authenticationService.Object));

        Assert.Equal("Cannot update row. Existing row with number 11 could not be found.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        var newRow = GetRowModel(10);

        var ex = Assert.Throws<DomainLogicException>(() => booking.DeleteRow(10, authenticationService.Object));

        Assert.Equal("Changes can only be made when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Row_Number()
    {
        var booking = CommonTestValues.GetBooking();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(booking.Commissioner.UserId!);
        var newRow = GetRowModel(10);
        var ex = Assert.Throws<NotFoundException>(() => booking.DeleteRow(11, authenticationService.Object));

        Assert.Equal("Cannot delete row. Existing row with number 11 could not be found.", ex.Message);
    }

    [Fact]
    public void Set_Booking_Status_Cancelled()
    {
        const string commissioner = "MIHSTE";
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner);

        booking.SetCancelledStatus(authenticationService.Object);

        Assert.Equal(WiniStatus.Cancelled, booking.BookingStatus.Status);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status.Status == WiniStatus.Cancelled);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_Cancelled_Not_Commissioner()
    {
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns("MIHSTE");
        var booking = CommonTestValues.GetBooking();

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetCancelledStatus(authenticationService.Object));

        Assert.Equal("Only commissioners can change status to Cancelled.", ex.Message);
        Assert.Equal("MIHSTE", ex.AttemptedValue);
        Assert.Equal("userId", ex.PropertyName);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_Cancelled_Wrong_Status()
    {
        const string commissioner = "MIHSTE";
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.Cancelled);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetCancelledStatus(authenticationService.Object));

        Assert.Equal("Status cannot be Sent or Cancelled", ex.Message);
    }

    [Fact]
    public void Set_Booking_Status_SendError()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeSent);

        booking.SetSendErrorStatus(authenticationService.Object, authorizationService.Object);

        Assert.Equal(WiniStatus.SendError, booking.BookingStatus.Status);
        var allRowUnauthorized = booking.Rows.All(_ => !_.Authorizer.HasAuthorized);
        Assert.True(allRowUnauthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.SendError);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_SendError_Not_Admin()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeSent);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSendErrorStatus(authenticationService.Object, authorizationService.Object));

        Assert.Equal("Only admins can change status to SendError.", ex.Message);
        Assert.Null(ex.AttemptedValue);
        Assert.Null(ex.PropertyName);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_SendError_Wrong_Status()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.Saved);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSendErrorStatus(authenticationService.Object, authorizationService.Object));

        Assert.Equal("Status cannot be anything other than ToBeSent", ex.Message);
        Assert.Equal("SendError", ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public void Set_Booking_Status_NotAuthorizedOnTime()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer("XMIHST", false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        booking.SetNotAuthorizedOnTimeStatus(authenticationService.Object, authorizationService.Object, new DateTime(2023, 3, 23, 23, 1, 0));

        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.NotAuthorizedOnTime);
        Assert.True(hasHistorySaved);
        Assert.Equal(2, booking.DomainEvents.Count);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status.Status == WiniStatus.NotAuthorizedOnTime);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status.Status == WiniStatus.Saved);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_NotAuthorizedOnTime_Three_Days_Not_Passed()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeAuthorized);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authenticationService.Object, authorizationService.Object, new DateTime(2023, 3, 23, 22, 59, 0)));

        Assert.Equal("Status cannot be changed to NotAuthorizedOnTime. 72 hours have not passed yet.", ex.Message);
        Assert.Equal("2023-03-23 22:59:00", ex.AttemptedValue);
        Assert.Equal("now", ex.PropertyName);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_NotAuthorizedOnTime_Not_Admin()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeAuthorized);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authenticationService.Object, authorizationService.Object, new DateTime(2023, 3, 23, 23, 59, 0)));

        Assert.Equal("Only admins can change status to NotAuthorizedOnTime.", ex.Message);
        Assert.Null(ex.AttemptedValue);
        Assert.Null(ex.PropertyName);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_NotAuthorizedOnTime_Wrong_Status()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeSent);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authenticationService.Object, authorizationService.Object, new DateTime(2023, 3, 23, 23, 59, 0)));

        Assert.Equal("Status cannot be anything other than ToBeAuthorized", ex.Message);
        Assert.Equal("NotAuthorizedOnTime", ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public void Set_Booking_Status_Saved_Commissioner()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer("XMIHST", true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeSent);

        booking.SetSavedStatus(authenticationService.Object, authorizationService.Object);

        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeSent);
        Assert.True(hasHistorySaved);
        var allRowUnauthorized = booking.Rows.All(_ => !_.Authorizer.HasAuthorized);
        Assert.True(allRowUnauthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.Saved);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public void Set_Booking_Status_Saved_Authorizer()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeSent);

        booking.SetSavedStatus(authenticationService.Object, authorizationService.Object);

        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeSent);
        Assert.True(hasHistorySaved);
        var allRowUnauthorized = booking.Rows.All(_ => !_.Authorizer.HasAuthorized);
        Assert.True(allRowUnauthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.Saved);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public void Set_Booking_Status_Saved_Admin()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(false);

        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns("ADMIN");
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeSent);

        booking.SetSavedStatus(authenticationService.Object, authorizationService.Object);

        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeSent);
        Assert.True(hasHistorySaved);
        var allRowUnauthorized = booking.Rows.All(_ => !_.Authorizer.HasAuthorized);
        Assert.True(allRowUnauthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.Saved);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_Saved_Unknown_User()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns("USERID");

        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeSent);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSavedStatus(authenticationService.Object, authorizationService.Object));

        Assert.Equal("Only admins, commissioners or authorizers can change status to Saved.", ex.Message);
        Assert.Null(ex.AttemptedValue);
        Assert.Null(ex.PropertyName);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_Saved_Wrong_Status()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);

        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.Saved);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSavedStatus(authenticationService.Object, authorizationService.Object));

        Assert.Equal("Status cannot be Sent, Saved or Cancelled", ex.Message);
        Assert.Equal("Saved", ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeAuthorized()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var validationService = GetBookingValidationService();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.Saved);

        await booking.SetToBeAuthorizedStatusAsync(authenticationService.Object, validationService, _companies);

        Assert.Equal(WiniStatus.ToBeAuthorized, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.Saved);
        Assert.True(hasHistorySaved);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status.Status == WiniStatus.ToBeAuthorized);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeAuthorized_Not_Valid_Error()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var validationService = GetBookingValidationService();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-50, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.Saved);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() => booking.SetToBeAuthorizedStatusAsync(authenticationService.Object, validationService, _companies));

        Assert.Equal("Failed to validate booking 1.", ex.Message);
        Assert.NotNull(ex.Errors);
        Assert.Single(ex.Errors);
        Assert.Contains(ex.Errors, _ => _.PropertyName == "Debit & Credit" && _.Message == "Debit and credit must be equal when using currency code 'SEK'. Balance = 50");
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeAuthorized_Wrong_Status()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var validationService = GetBookingValidationService();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(commissioner);

        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeSent);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(() => booking.SetToBeAuthorizedStatusAsync(authenticationService.Object, validationService, _companies));

        Assert.Equal("Status cannot be anything other than Saved", ex.Message);
        Assert.Equal("ToBeAuthorized", ex.AttemptedValue);
        Assert.Equal("Status", ex.PropertyName);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeSent()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var validationService = GetBookingValidationService();
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer, false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        await booking.SetToBeSentStatusAsync(
            authenticationService.Object,
            authorizationService.Object,
            validationService,
            _companies
        );

        Assert.Equal(WiniStatus.ToBeSent, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeAuthorized);
        Assert.True(hasHistorySaved);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.ToBeSent);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeSent_Multiple_Authorizers()
    {
        const string commissioner = "MIHSTE";
        const string authorizer1 = "XMIHST";
        const string authorizer2 = "MIHHST";
        var validationService = GetBookingValidationService();
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer1);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer1, false),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("11000"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer2, false),
                new Money(1000, "SEK", 0)
            ),
            new(
                4,
                new BusinessUnit("100KKTOT"),
                new Account("25010"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-1000, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        await booking.SetToBeSentStatusAsync(
            authenticationService.Object,
            authorizationService.Object,
            validationService,
            _companies
        );

        Assert.Equal(WiniStatus.ToBeAuthorized, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeAuthorized);
        Assert.True(hasHistorySaved);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer1 && _.Authorizer.HasAuthorized);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer2 && !_.Authorizer.HasAuthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.ToBeAuthorized);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeSent_Multiple_Authorizers_Partly_Authorized()
    {
        const string commissioner = "MIHSTE";
        const string authorizer1 = "XMIHST";
        const string authorizer2 = "MIHHST";
        var validationService = GetBookingValidationService();
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer2);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer1, true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("11000"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer2, false),
                new Money(1000, "SEK", 0)
            ),
            new(
                4,
                new BusinessUnit("100KKTOT"),
                new Account("25010"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-1000, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        await booking.SetToBeSentStatusAsync(
            authenticationService.Object,
            authorizationService.Object,
            validationService,
            _companies
        );

        Assert.Equal(WiniStatus.ToBeSent, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.ToBeAuthorized);
        Assert.True(hasHistorySaved);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer1 && _.Authorizer.HasAuthorized);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer2 && _.Authorizer.HasAuthorized);
        Assert.NotEmpty(booking.DomainEvents);
        var statusEvent = booking.DomainEvents.SingleOrDefault(_ => _ is WiniStatusEvent evt && evt.Status.Status == WiniStatus.ToBeSent);
        Assert.NotNull(statusEvent);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeSent_Multiple_Authorizers_Nothing_Left_To_Authorize_For_User()
    {
        const string commissioner = "MIHSTE";
        const string authorizer1 = "XMIHST";
        const string authorizer2 = "MIHHST";
        var validationService = GetBookingValidationService();
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer1);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer1, true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("11000"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer2, false),
                new Money(1000, "SEK", 0)
            ),
            new(
                4,
                new BusinessUnit("100KKTOT"),
                new Account("25010"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-1000, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(async () => await booking.SetToBeSentStatusAsync(
            authenticationService.Object,
            authorizationService.Object,
            validationService,
            _companies
        ));

        Assert.NotNull(ex);
        Assert.Equal("There are no rows to authorize for user.", ex.Message);
        Assert.Equal(WiniStatus.ToBeAuthorized, booking.BookingStatus.Status);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer1 && _.Authorizer.HasAuthorized);
        Assert.Contains(booking.Rows, _ => _.Authorizer.UserId == authorizer2 && !_.Authorizer.HasAuthorized);
        Assert.Empty(booking.DomainEvents);
    }

    [Fact]
    public async Task Set_Booking_Status_ToBeSent_Multiple_Authorizers_Nothing_All_Rows_Authorized()
    {
        const string commissioner = "MIHSTE";
        const string authorizer1 = "XMIHST";
        const string authorizer2 = "MIHHST";
        var validationService = GetBookingValidationService();
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(true);
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(authorizer1);
        var rows = new List<BookingRow> {
            new(
                1,
                new BusinessUnit("100KKTOT"),
                new Account("10500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer1, true),
                new Money(100, "SEK", 0)
            ),
            new(
                2,
                new BusinessUnit("100KKTOT"),
                new Account("24500"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-100, "SEK", 0)
            ),
            new(
                3,
                new BusinessUnit("100KKTOT"),
                new Account("11000"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark("Test"),
                new Authorizer(authorizer2, true),
                new Money(1000, "SEK", 0)
            ),
            new(
                4,
                new BusinessUnit("100KKTOT"),
                new Account("25010"),
                new Subledger(),
                new CostObject(1),
                new CostObject(2),
                new CostObject(3),
                new CostObject(4),
                new Remark(),
                new Authorizer(),
                new Money(-1000, "SEK", 0)
            )
        };
        var booking = CommonTestValues.GetBooking(rows, commissioner, WiniStatus.ToBeAuthorized);

        var ex = await Assert.ThrowsAsync<DomainLogicException>(async () => await booking.SetToBeSentStatusAsync(
            authenticationService.Object,
            authorizationService.Object,
            validationService,
            _companies
        ));

        Assert.NotNull(ex);
        Assert.Equal("All booking rows are already authorized.", ex.Message);
    }

    [Theory]
    [InlineData("MIHSTE", "MIHSTE", false, true, default)]// Commisioner
    [InlineData("XMIHST", "MIHSTE", true, true, default)]// Admin
    [InlineData("XMIHST", "MIHSTE", false, false, "Forbidden")]// Neither
    public void Can_Delete_Booking_Commissioner(string commissioner, string user, bool isAdmin, bool expectedResult, string? expectedReason)
    {
        var booking = CommonTestValues.GetBooking(commissioner);
        var newRow = GetRowModel();
        var authenticationService = new Mock<IAuthenticationService>();
        authenticationService.Setup(_ => _.GetUserId()).Returns(user);
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(isAdmin);

        var (canDelete, reason) = booking.CanDeleteBooking(authenticationService.Object, authorizationService.Object);

        Assert.Equal(expectedResult, canDelete);
        Assert.Equal(expectedReason, reason);
    }

    private static BookingRowModel GetRowModel(int rowNo = 1, decimal amount = 100)
    => new()
    {
        RowNumber = rowNo,
        Account = "12345",
        Amount = amount,
        Authorizer = "MIHSTE",
        BusinessUnit = "100KKTOT8888",
        CostObject1 = "CO1",
        CostObject2 = "CO2",
        CostObject3 = "CO3",
        CostObject4 = "CO4",
        CostObjectType1 = "A",
        CostObjectType2 = "B",
        CostObjectType3 = "C",
        CostObjectType4 = default,
        CurrencyCode = "EUR",
        ExchangeRate = 10,
        Remark = "TEST",
        Subledger = "990099",
        SubledgerType = "A",
        Subsidiary = "7894"
    };

    private static BookingValidationService GetBookingValidationService(
        bool isAdmin = false,
        bool isAuthNeeded = true,
        bool isError = false
    )
    {
        var res = isError
            ? (
                false,
                new[] { new ValidationError {
                    Message = "Error has occurred",
                    PropertyName = "Test",
                    ErrorCode = "1234",
                    AttemptedValue = "1234"
                } }
            )
            : (true, Array.Empty<ValidationError>());
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(isAdmin);
        authorizationService.Setup(_ => _.IsBookingAuthorizationNeeded()).Returns(isAuthNeeded);
        var authorizerValidationService = new Mock<IAuthorizerValidationService>();
        authorizerValidationService.Setup(_ => _.CanAuthorizeBookingRows(It.IsAny<IEnumerable<BookingRow>>()))
            .ReturnsAsync(res);
        var bookingPeriodValidationService = new Mock<IBookingPeriodValidationService>();
        bookingPeriodValidationService.Setup(_ => _.ValidateAsync(It.IsAny<Booking>()))
            .ReturnsAsync((true, Array.Empty<ValidationError>()));
        var accountingValidationService = new Mock<IAccountingValidationService>();
        accountingValidationService.Setup(_ => _.ValidateAsync(It.IsAny<IEnumerable<AccountingValidationInputModel>>()))
            .ReturnsAsync((true, Array.Empty<ValidationError>()));
        return new(
            authorizationService.Object,
            authorizerValidationService.Object,
            bookingPeriodValidationService.Object,
            accountingValidationService.Object
        );
    }
}