namespace Tests.UnitTests.Wini;

public class BookingRowTests
{
    private static readonly Company[] _companies = CommonTestValues.GetCompanies();

    [Fact]
    public void Create_New_Booking_Row_With_No_Data()
    {
        var bookingRow = new BookingRow(default);

        Assert.Equal(default, bookingRow.Account.Value);
        Assert.Equal(default, bookingRow.Account.Subsidiary);
        Assert.Equal(0, bookingRow.Money.Amount);
        Assert.Equal(default, bookingRow.Money.Currency.CurrencyRate);
        Assert.Equal(default, bookingRow.Money.Currency.CurrencyCode.Code);
        Assert.Equal("", bookingRow.BusinessUnit.ToString());
        Assert.Equal(default, bookingRow.BusinessUnit.CompanyCode.Code);
        Assert.Equal(default, bookingRow.BusinessUnit.Costcenter.Code);
        Assert.Equal(default, bookingRow.BusinessUnit.Product.Code);
        Assert.Equal(default, bookingRow.Authorizer.UserId);
        Assert.Equal(default, bookingRow.Authorizer.HasAuthorized);
        Assert.Equal(1, bookingRow.CostObject1.Number);
        Assert.Equal(default, bookingRow.CostObject1.Type);
        Assert.Equal(default, bookingRow.CostObject1.Value);
        Assert.Equal(2, bookingRow.CostObject2.Number);
        Assert.Equal(default, bookingRow.CostObject2.Type);
        Assert.Equal(default, bookingRow.CostObject2.Value);
        Assert.Equal(3, bookingRow.CostObject3.Number);
        Assert.Equal(default, bookingRow.CostObject3.Type);
        Assert.Equal(default, bookingRow.CostObject3.Value);
        Assert.Equal(4, bookingRow.CostObject4.Number);
        Assert.Equal(default, bookingRow.CostObject4.Type);
        Assert.Equal(default, bookingRow.CostObject4.Value);
        Assert.Equal(default, bookingRow.Remark.Text);
        Assert.Equal(default, bookingRow.Subledger.Type);
        Assert.Equal(default, bookingRow.Subledger.Value);
        Assert.Equal(default, bookingRow.RowNumber);
    }

    [Fact]
    public void Create_Booking_Row_With_Data()
    {
        const int rowNumber = 1;
        var account = new Account("12345", "1234");
        var currency = new Currency(new CurrencyCode("SEK"), 10);
        var amount = new Money(100, currency);
        var bu = new BusinessUnit("100KKTOT1234");
        var authorizer = new Authorizer("MIHSTE", false);
        var costObject1 = new CostObject(1, "XYZ", "A");
        var costObject2 = new CostObject(2);
        var costObject3 = new CostObject(3);
        var costObject4 = new CostObject(4);
        var remark = new Remark("Test");
        var subledger = new Subledger("ASDF", "A");
        var bookingRow = new BookingRow(
            rowNumber,
            bu,
            account,
            subledger,
            costObject1,
            costObject2,
            costObject3,
            costObject4,
            remark,
            authorizer,
            amount
        );

        Assert.Equal(account.Value, bookingRow.Account.Value);
        Assert.Equal(account.Subsidiary, bookingRow.Account.Subsidiary);
        Assert.Equal(amount.Amount, bookingRow.Money.Amount);
        Assert.Equal(amount.Currency.CurrencyRate, bookingRow.Money.Currency.CurrencyRate);
        Assert.Equal(amount.Currency.CurrencyCode.Code, bookingRow.Money.Currency.CurrencyCode.Code);
        Assert.Equal("100KKTOT1234", bookingRow.BusinessUnit.ToString());
        Assert.Equal(bu.CompanyCode.Code, bookingRow.BusinessUnit.CompanyCode.Code);
        Assert.Equal(bu.Costcenter.Code, bookingRow.BusinessUnit.Costcenter.Code);
        Assert.Equal(bu.Product.Code, bookingRow.BusinessUnit.Product.Code);
        Assert.Equal(authorizer.UserId, bookingRow.Authorizer.UserId);
        Assert.Equal(authorizer.HasAuthorized, bookingRow.Authorizer.HasAuthorized);
        Assert.Equal(costObject1.Number, bookingRow.CostObject1.Number);
        Assert.Equal(costObject1.Type, bookingRow.CostObject1.Type);
        Assert.Equal(costObject1.Value, bookingRow.CostObject1.Value);
        Assert.Equal(costObject2.Number, bookingRow.CostObject2.Number);
        Assert.Equal(costObject2.Type, bookingRow.CostObject2.Type);
        Assert.Equal(costObject2.Value, bookingRow.CostObject2.Value);
        Assert.Equal(costObject3.Number, bookingRow.CostObject3.Number);
        Assert.Equal(costObject3.Type, bookingRow.CostObject3.Type);
        Assert.Equal(costObject3.Value, bookingRow.CostObject3.Value);
        Assert.Equal(costObject4.Number, bookingRow.CostObject4.Number);
        Assert.Equal(costObject4.Type, bookingRow.CostObject4.Type);
        Assert.Equal(costObject4.Value, bookingRow.CostObject4.Value);
        Assert.Equal(remark.Text, bookingRow.Remark.Text);
        Assert.Equal(subledger.Type, bookingRow.Subledger.Type);
        Assert.Equal(subledger.Value, bookingRow.Subledger.Value);
        Assert.Equal(rowNumber, bookingRow.RowNumber);
    }

    [Fact]
    public void Change_Booking_Row_Authorization()
    {
        const int rowNumber = 1;
        var account = new Account("12345", "1234");
        var currency = new Currency(new CurrencyCode("SEK"), 10);
        var amount = new Money(100, currency);
        var bu = new BusinessUnit("100KKTOT1234");
        var authorizer = new Authorizer("MIHSTE", false);
        var costObject1 = new CostObject(1, "XYZ", "A");
        var costObject2 = new CostObject(2);
        var costObject3 = new CostObject(3);
        var costObject4 = new CostObject(4);
        var remark = new Remark("Test");
        var subledger = new Subledger("ASDF", "A");
        var bookingRow = new BookingRow(
            rowNumber,
            bu,
            account,
            subledger,
            costObject1,
            costObject2,
            costObject3,
            costObject4,
            remark,
            authorizer,
            amount
        );

        var changedRow = bookingRow.ChangeRowAuthorization(true);

        Assert.Equal(changedRow.Account.Value, bookingRow.Account.Value);
        Assert.Equal(changedRow.Account.Subsidiary, bookingRow.Account.Subsidiary);
        Assert.Equal(changedRow.Money.Amount, bookingRow.Money.Amount);
        Assert.Equal(changedRow.Money.Currency.CurrencyRate, bookingRow.Money.Currency.CurrencyRate);
        Assert.Equal(changedRow.Money.Currency.CurrencyCode.Code, bookingRow.Money.Currency.CurrencyCode.Code);
        Assert.Equal(changedRow.BusinessUnit.ToString(), bookingRow.BusinessUnit.ToString());
        Assert.Equal(changedRow.BusinessUnit.CompanyCode.Code, bookingRow.BusinessUnit.CompanyCode.Code);
        Assert.Equal(changedRow.BusinessUnit.Costcenter.Code, bookingRow.BusinessUnit.Costcenter.Code);
        Assert.Equal(changedRow.BusinessUnit.Product.Code, bookingRow.BusinessUnit.Product.Code);
        Assert.Equal(changedRow.Authorizer.UserId, bookingRow.Authorizer.UserId);
        Assert.NotEqual(changedRow.Authorizer.HasAuthorized, bookingRow.Authorizer.HasAuthorized);
        Assert.Equal(changedRow.CostObject1.Number, bookingRow.CostObject1.Number);
        Assert.Equal(changedRow.CostObject1.Type, bookingRow.CostObject1.Type);
        Assert.Equal(changedRow.CostObject1.Value, bookingRow.CostObject1.Value);
        Assert.Equal(changedRow.CostObject2.Number, bookingRow.CostObject2.Number);
        Assert.Equal(changedRow.CostObject2.Type, bookingRow.CostObject2.Type);
        Assert.Equal(changedRow.CostObject2.Value, bookingRow.CostObject2.Value);
        Assert.Equal(changedRow.CostObject3.Number, bookingRow.CostObject3.Number);
        Assert.Equal(changedRow.CostObject3.Type, bookingRow.CostObject3.Type);
        Assert.Equal(changedRow.CostObject3.Value, bookingRow.CostObject3.Value);
        Assert.Equal(changedRow.CostObject4.Number, bookingRow.CostObject4.Number);
        Assert.Equal(changedRow.CostObject4.Type, bookingRow.CostObject4.Type);
        Assert.Equal(changedRow.CostObject4.Value, bookingRow.CostObject4.Value);
        Assert.Equal(changedRow.Remark.Text, bookingRow.Remark.Text);
        Assert.Equal(changedRow.Subledger.Type, bookingRow.Subledger.Type);
        Assert.Equal(changedRow.Subledger.Value, bookingRow.Subledger.Value);
        Assert.Equal(changedRow.RowNumber, bookingRow.RowNumber);
    }

    [Fact]
    public void Create_Booking_Row_Change_Values()
    {
        const int rowNumber = 1;
        var bookingRow = new BookingRow(rowNumber);

        Assert.Equal(default, bookingRow.Account.Value);
        Assert.Equal(default, bookingRow.Account.Subsidiary);
        Assert.Equal(0, bookingRow.Money.Amount);
        Assert.Equal(default, bookingRow.Money.Currency.CurrencyRate);
        Assert.Equal(default, bookingRow.Money.Currency.CurrencyCode.Code);
        Assert.Equal("", bookingRow.BusinessUnit.ToString());
        Assert.Equal(default, bookingRow.BusinessUnit.CompanyCode.Code);
        Assert.Equal(default, bookingRow.BusinessUnit.Costcenter.Code);
        Assert.Equal(default, bookingRow.BusinessUnit.Product.Code);
        Assert.Equal(default, bookingRow.Authorizer.UserId);
        Assert.Equal(default, bookingRow.Authorizer.HasAuthorized);
        Assert.Equal(1, bookingRow.CostObject1.Number);
        Assert.Equal(default, bookingRow.CostObject1.Type);
        Assert.Equal(default, bookingRow.CostObject1.Value);
        Assert.Equal(2, bookingRow.CostObject2.Number);
        Assert.Equal(default, bookingRow.CostObject2.Type);
        Assert.Equal(default, bookingRow.CostObject2.Value);
        Assert.Equal(3, bookingRow.CostObject3.Number);
        Assert.Equal(default, bookingRow.CostObject3.Type);
        Assert.Equal(default, bookingRow.CostObject3.Value);
        Assert.Equal(4, bookingRow.CostObject4.Number);
        Assert.Equal(default, bookingRow.CostObject4.Type);
        Assert.Equal(default, bookingRow.CostObject4.Value);
        Assert.Equal(default, bookingRow.Remark.Text);
        Assert.Equal(default, bookingRow.Subledger.Type);
        Assert.Equal(default, bookingRow.Subledger.Value);
        Assert.Equal(rowNumber, bookingRow.RowNumber);
    }

    [Theory]
    [InlineData("100KKTOT1234", true)]
    [InlineData("200KKTOT1234", true)]
    [InlineData("999KKTOT1234", false)]
    public void Check_Is_Company_Code_Is_Allowed(string businessUnit, bool expectedResult)
    {
        const int rowNumber = 1;
        var account = new Account("12345", "1234");
        var currency = new Currency(new CurrencyCode("SEK"), 10);
        var amount = new Money(100, currency);
        var bu = new BusinessUnit(businessUnit);
        var authorizer = new Authorizer("MIHSTE", false);
        var costObject1 = new CostObject(1, "XYZ", "A");
        var costObject2 = new CostObject(2);
        var costObject3 = new CostObject(3);
        var costObject4 = new CostObject(4);
        var remark = new Remark("Test");
        var subledger = new Subledger("ASDF", "A");
        var bookingRow = new BookingRow(
            rowNumber,
            bu,
            account,
            subledger,
            costObject1,
            costObject2,
            costObject3,
            costObject4,
            remark,
            authorizer,
            amount
        );

        var result = bookingRow.IsCompanyCodeAllowed(_companies);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("100KKTOT1234", "SEK", true)]
    [InlineData("100KKTOT1234", "DKK", false)]
    [InlineData("200KKTOT1234", "NOK", true)]
    [InlineData("200KKTOT1234", "EUR", false)]
    [InlineData("300KKTOT1234", "DKK", true)]
    [InlineData("300KKTOT1234", "EUR", false)]
    [InlineData("400KKTOT1234", "EUR", true)]
    [InlineData("400KKTOT1234", "SEK", false)]
    public void Check_Is_Base_Currency_Used(string businessUnit, string currencyCode, bool expectedResult)
    {
        const int rowNumber = 1;
        var account = new Account("12345", "1234");
        var currency = new Currency(new CurrencyCode(currencyCode), 10);
        var amount = new Money(100, currency);
        var bu = new BusinessUnit(businessUnit);
        var authorizer = new Authorizer("MIHSTE", false);
        var costObject1 = new CostObject(1, "XYZ", "A");
        var costObject2 = new CostObject(2);
        var costObject3 = new CostObject(3);
        var costObject4 = new CostObject(4);
        var remark = new Remark("Test");
        var subledger = new Subledger("ASDF", "A");
        var bookingRow = new BookingRow(
            rowNumber,
            bu,
            account,
            subledger,
            costObject1,
            costObject2,
            costObject3,
            costObject4,
            remark,
            authorizer,
            amount
        );

        var result = bookingRow.IsBaseCurrencyUsed(_companies);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("MIHSTE", "MIHSTE", 100, false, true)]
    [InlineData("MIHSTE", "MIHSTE", -100, false, false)]
    [InlineData("MIHSTE", "MIHSTE", 100, true, false)]
    [InlineData("MIHSTE", "XMIHST", 100, false, false)]
    [InlineData("MIHSTE", "", 100, false, false)]
    public void Check_Can_Row_Be_Authorized_By_User(
        string authorizerUserId,
        string userToAuthorizeUserId,
        decimal money,
        bool isRowAuthorized,
        bool expectedResult)
    {
        const int rowNumber = 1;
        var account = new Account("12345", "1234");
        var currency = new Currency(new CurrencyCode("SEK"), 10);
        var amount = new Money(money, currency);
        var bu = new BusinessUnit("100KKTOT1234");
        var authorizer = new Authorizer(authorizerUserId, isRowAuthorized);
        var costObject1 = new CostObject(1, "XYZ", "A");
        var costObject2 = new CostObject(2);
        var costObject3 = new CostObject(3);
        var costObject4 = new CostObject(4);
        var remark = new Remark("Test");
        var subledger = new Subledger("ASDF", "A");
        var bookingRow = new BookingRow(
            rowNumber,
            bu,
            account,
            subledger,
            costObject1,
            costObject2,
            costObject3,
            costObject4,
            remark,
            authorizer,
            amount
        );

        var result = bookingRow.CanRowBeAuthorizedByUser(userToAuthorizeUserId);

        Assert.Equal(expectedResult, result);
    }
}