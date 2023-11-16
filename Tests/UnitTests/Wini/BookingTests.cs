namespace Tests.UnitTests.Wini;

public class BookingTests
{
    [Fact]
    public void Create_New_Booking_With_No_Data()
    {
        var booking = GetNewEmptyBooking();

        var today = DateTime.UtcNow.Date;
        Assert.Equal(default, booking.BookingId);
        Assert.Equal(today, booking.Created.Date);
        Assert.Equal(today, booking.BookingDate.Date);
        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        Assert.Equal(today, booking.BookingStatus.Updated.Date);
        Assert.Equal("MIHSTE", booking.Commissioner.UserId);
        Assert.False(booking.IsReversed);
        Assert.Equal(Ledgers.AA, booking.LedgerType.Type);
        Assert.Equal(default, booking.TextToE1.Text);
        Assert.Empty(booking.Rows);
        Assert.Empty(booking.Attachments);
        Assert.Empty(booking.Comments);
        Assert.Empty(booking.Messages);
    }

    [Fact]
    public void Create_New_Booking_With_Data()
    {
        var booking = GetBooking();

        var today = DateTime.UtcNow.Date;
        Assert.Equal(1, booking.BookingId?.Value);
        Assert.Equal(new DateTime(2023, 3, 20, 23, 0, 0), booking.Created);
        Assert.Equal(new DateTime(2023, 3, 22), booking.BookingDate.Date);
        Assert.Equal(WiniStatus.Saved, booking.BookingStatus.Status);
        Assert.Equal(new DateTime(2023, 3, 24), booking.BookingStatus.Updated);
        Assert.Equal("XMIHST", booking.Commissioner.UserId);
        Assert.True(booking.IsReversed);
        Assert.Equal(Ledgers.GP, booking.LedgerType.Type);
        Assert.Equal("Test", booking.TextToE1.Text);
        Assert.Empty(booking.Rows);
        Assert.Empty(booking.Attachments);
        Assert.Empty(booking.Comments);
        Assert.Empty(booking.Messages);
    }

    [Fact]
    public void Add_New_Row_To_Empty_Booking()
    {
        var booking = GetNewEmptyBooking();
        var newRow = GetRowModel();

        booking.AddNewRow(newRow);

        Assert.Single(booking.Rows);
        var row = booking.Rows.Single();
        Assert.Equal(default, row.BookingId?.Value);
        Assert.Equal("12345", row.Account.Value);
        Assert.Equal("7894", row.Account.Subsidiary);
        Assert.Equal(100, row.Money.Amount);
        Assert.Equal("EUR", row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(10, row.Money.Currency.CurrencyRate);
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
        var booking = GetBooking();
        var newRow = GetRowModel();

        booking.AddNewRow(newRow);

        Assert.Single(booking.Rows);
        var row = booking.Rows.Single();
        Assert.Equal(1, row.BookingId?.Value);
        Assert.Equal("12345", row.Account.Value);
        Assert.Equal("7894", row.Account.Subsidiary);
        Assert.Equal(100, row.Money.Amount);
        Assert.Equal("EUR", row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(10, row.Money.Currency.CurrencyRate);
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
        var booking = GetBooking();
        var newRow = GetRowModel(10);
        booking.AddNewRow(newRow);

        var editRow = GetRowModel(10);
        editRow.Account = "98765";
        editRow.Subsidiary = "6543";
        editRow.Amount = 200;
        editRow.CurrencyCode = "SEK";
        editRow.CurrencyRate = -10;
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
        Assert.Equal(editRow.Id, row.RowId?.Value);
        Assert.Equal(1, row.BookingId?.Value);
        Assert.Equal(editRow.Account, row.Account.Value);
        Assert.Equal(editRow.Subsidiary, row.Account.Subsidiary);
        Assert.Equal(editRow.Amount, row.Money.Amount);
        Assert.Equal(editRow.CurrencyCode, row.Money.Currency.CurrencyCode.Code);
        Assert.Equal(editRow.CurrencyRate, row.Money.Currency.CurrencyRate);
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
        var booking = GetBooking();
        var newRow = GetRowModel(10);
        booking.AddNewRow(newRow);

        booking.DeleteRow(10);

        Assert.Empty(booking.Rows);
    }

    [Fact]
    public void Fail_To_Add_New_Row_Wrong_Status()
    {
        var booking = GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel();

        var ex = Assert.Throws<DomainLogicException>(() => booking.AddNewRow(newRow));

        Assert.Equal("Cannot add new row. Rows can only be added when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Status()
    {
        var booking = GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel();

        var ex = Assert.Throws<DomainLogicException>(() => booking.EditRow(newRow));

        Assert.Equal("Cannot edit row. Rows can only be added when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Edit_Row_Wrong_Id()
    {
        var booking = GetBooking();
        var newRow = GetRowModel(10);
        var editRow = GetRowModel(11);
        var ex = Assert.Throws<DomainLogicException>(() => booking.EditRow(editRow));

        Assert.Equal("Cannot edit row. Existing row with ID 11 could not be found.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Status()
    {
        var booking = GetBooking(status: WiniStatus.Cancelled);
        var newRow = GetRowModel(10);

        var ex = Assert.Throws<DomainLogicException>(() => booking.DeleteRow(10));

        Assert.Equal("Cannot delete row. Rows can only be deleted when Booking status is 'Saved'.", ex.Message);
    }

    [Fact]
    public void Fail_To_Delete_Row_Wrong_Id()
    {
        var booking = GetBooking();
        var newRow = GetRowModel(10);
        var ex = Assert.Throws<DomainLogicException>(() => booking.DeleteRow(11));

        Assert.Equal($"Cannot delete row. Existing row with ID 11 could not be found.", ex.Message);
    }

    private Booking GetNewEmptyBooking()
    => new(
        default,
        new Commissioner("MIHSTE")
    );

    private Booking GetBooking(string commissioner = "XMIHST", WiniStatus status = WiniStatus.Saved)
    => new(
        new IdValue<int>(1),
        new BookingStatus(status, new DateTime(2023, 3, 24)),
        new Commissioner(commissioner),
        new BookingDate(new DateTime(2023, 3, 22)),
        new TextToE1("Test"),
        true,
        new LedgerType(Ledgers.GP),
        new List<BookingRow>(),
        new List<Comment>(),
        new List<RecipientMessage>(),
        new List<Attachment>(),
        new DateTime(2023, 3, 20, 23, 0, 0)
    );

    private BookingRowModel GetRowModel(int? id = default)
    => new()
    {
        Id = id,
        Account = "12345",
        Amount = 100,
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
        CurrencyRate = 10,
        Remark = "TEST",
        Subledger = "990099",
        SubledgerType = "A",
        Subsidiary = "7894"
    };
}