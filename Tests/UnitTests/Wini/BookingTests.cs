using Domain.Interfaces;
using Moq;

namespace Tests.UnitTests.Wini;

public class BookingTests
{
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

        booking.EditBookingHeader(header);

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

        booking.AddNewRow(newRow);

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
    public void Add_New_Row_To_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel();

        booking.AddNewRow(newRow);

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
        booking.AddNewRow(newRow);

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

        booking.EditRow(editRow);

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
    }

    [Fact]
    public void Delete_Row_Of_Existing_Booking()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(10);
        booking.AddNewRow(newRow);

        booking.DeleteRow(10);

        Assert.Empty(booking.Rows);
    }

    [Fact]
    public void Fail_To_Add_New_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel();

        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRow(newRow));

        Assert.Equal("Cannot add new row. Rows can only be added when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Add_New_Row_Duplicate_Row_Number()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Saved);
        var newRow = GetRowModel(1);
        booking.AddNewRow(newRow);
        newRow = GetRowModel(1);

        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRow(newRow));

        Assert.Equal("Cannot add new row. Row number 1 already exists.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel();

        var ex = Assert.Throws<DomainLogicException>(() => booking.EditRow(newRow));

        Assert.Equal("Cannot edit row. Rows can only be added when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Row_Number()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(10);
        var editRow = GetRowModel(11);
        var ex = Assert.Throws<NotFoundException>(() => booking.EditRow(editRow));

        Assert.Equal("Cannot edit row. Existing row with number 11 could not be found.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Status()
    {
        var booking = CommonTestValues.GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel(10);

        var ex = Assert.Throws<DomainLogicException>(() => booking.DeleteRow(10));

        Assert.Equal("Cannot delete row. Rows can only be deleted when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Row_Number()
    {
        var booking = CommonTestValues.GetBooking();
        var newRow = GetRowModel(10);
        var ex = Assert.Throws<NotFoundException>(() => booking.DeleteRow(11));

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
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.Cancelled);
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
                1,
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

        booking.SetSendErrorStatus(authorizationService.Object);

        Assert.Equal(WiniStatus.SendError, booking.BookingStatus.Status);
        var allRowUnauthorized = booking.Rows.All(_ => !_.Authorizer.HasAuthorized);
        Assert.True(allRowUnauthorized);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.SendError);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_SendError_Not_Admin()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeSent);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSendErrorStatus(authorizationService.Object));

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
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.Saved);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetSendErrorStatus(authorizationService.Object));

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
                1,
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

        booking.SetNotAuthorizedOnTimeStatus(authorizationService.Object, new DateTime(2023, 3, 23, 23, 1, 0));

        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        var hasHistorySaved = booking.BookingStatus.StatusHistory.Any(_ => _.Status == WiniStatus.NotAuthorizedOnTime);
        Assert.True(hasHistorySaved);
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.NotAuthorizedOnTime);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_NotAuthorizedOnTime_Three_Days_Not_Passed()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeAuthorized);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authorizationService.Object, new DateTime(2023, 3, 23, 22, 59, 0)));

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
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeAuthorized);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authorizationService.Object, new DateTime(2023, 3, 23, 23, 59, 0)));

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
        var booking = CommonTestValues.GetBooking(commissioner, WiniStatus.ToBeSent);

        var ex = Assert.Throws<DomainLogicException>(() => booking.SetNotAuthorizedOnTimeStatus(authorizationService.Object, new DateTime(2023, 3, 23, 23, 59, 0)));

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
                1,
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
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.Saved);
    }

    [Fact]
    public void Set_Booking_Status_Saved_Authorizer()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
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
                1,
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
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.Saved);
    }

    [Fact]
    public void Set_Booking_Status_Saved_Admin()
    {
        const string commissioner = "MIHSTE";
        const string authorizer = "XMIHST";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(true);
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
                1,
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
        Assert.Single(booking.DomainEvents);
        Assert.Contains(booking.DomainEvents, _ => ((WiniStatusEvent)_).Status == WiniStatus.Saved);
    }

    [Fact]
    public void Fail_To_Set_Booking_Status_Saved_Unknown_User()
    {
        const string commissioner = "MIHSTE";
        var authorizationService = new Mock<IAuthorizationService>();
        authorizationService.Setup(_ => _.IsAdmin()).Returns(false);
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
}