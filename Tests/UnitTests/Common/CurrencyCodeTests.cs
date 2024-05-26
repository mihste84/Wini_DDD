namespace Tests.UnitTests.Common;

public class CurrencyCodeTests
{
    [Fact]
    public void Create_CurrencyCode()
    {
        var currency = new CurrencyCode("SEK");
        Assert.Equal("SEK", currency.Code);

        currency = new CurrencyCode();
        Assert.Equal(default, currency.Code);
    }

    [Theory]
    [InlineData("SE", "SEK")]
    [InlineData("NO", "NOK")]
    [InlineData("FI", "EUR")]
    [InlineData("DK", "DKK")]
    [InlineData("LT", "EUR")]
    [InlineData("LV", "EUR")]
    [InlineData("EE", "EUR")]
    public void Create_CurrencyCode_By_Country(string code, string expectedCurrency)
    {
        var country = new Country(code);
        var currency = new CurrencyCode(country);
        Assert.Equal(expectedCurrency, currency.Code);
    }

    [Fact]
    public void Fail_To_Create_CurrencyCode_Too_Long_Code()
    {
        const string code = "SEK  ";
        var ex = Assert.Throws<DomainValidationException>(() => new CurrencyCode(code));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(code, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("The length of 'Currency Code' must be 3 characters or fewer. You entered 5 characters.", error.Message);
    }
}