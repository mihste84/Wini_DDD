namespace Tests.UnitTests;

public class CurrencyCodeTests
{
    [Fact]
    public void Create_CurrencyCode()
    {
        var currency = new CurrencyCode("SEK");
        Assert.Equal("SEK", currency.Code);
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
        var ex = Assert.Throws<TextValidationException>(() => new CurrencyCode(code));

        Assert.Equal(code, ex.AttemptedValue);
        Assert.Equal("code", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Currency code cannot be longer than 3 characters", ex.Message);
    }
}