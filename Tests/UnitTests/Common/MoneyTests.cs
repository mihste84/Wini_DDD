namespace Tests.UnitTests.Common;

public class MoneyTests
{
    [Fact]
    public void Create_Money()
    {
        var currencyCode = new CurrencyCode("SEK");
        var currency = new Currency(currencyCode, 10);
        var money = new Money(100, currency);
        Assert.Equal(100, money.Amount);
        Assert.Equal("SEK", money.Currency.CurrencyCode.Code);
        Assert.Equal(10, money.Currency.ExchangeRate);

        money = new Money(200, "SEK", default);
        Assert.Equal(200, money.Amount);
        Assert.Equal("SEK", money.Currency.CurrencyCode.Code);
        Assert.Equal(default, money.Currency.ExchangeRate);

        money = new Money(200, "SEK", 10);
        Assert.Equal(200, money.Amount);
        Assert.Equal("SEK", money.Currency.CurrencyCode.Code);
        Assert.Equal(10, money.Currency.ExchangeRate);

        money = new Money(200, default, 10);
        Assert.Equal(200, money.Amount);
        Assert.Equal(default, money.Currency.CurrencyCode.Code);
        Assert.Equal(10, money.Currency.ExchangeRate);

        money = new Money();
        Assert.Equal(0, money.Amount);
        Assert.Equal(default, money.Currency.CurrencyCode.Code);
        Assert.Equal(default, money.Currency.ExchangeRate);
    }

    [Fact]
    public void Money_Is_Debit_Amount()
    {
        var currencyCode = new CurrencyCode("SEK");
        var currency = new Currency(currencyCode, 10);
        var money = new Money(100, currency);
        Assert.True(money.IsDebitRow());
        Assert.False(money.IsCreditRow());
    }

    [Fact]
    public void Money_Is_Credit_Amount()
    {
        var currencyCode = new CurrencyCode("SEK");
        var currency = new Currency(currencyCode, 10);
        var money = new Money(-100, currency);
        Assert.True(money.IsCreditRow());
        Assert.False(money.IsDebitRow());
    }
}