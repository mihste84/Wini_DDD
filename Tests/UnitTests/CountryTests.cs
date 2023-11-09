namespace Tests.UnitTests;

public class CountryTests
{
    [Theory]
    [InlineData("SE", "Sweden")]
    [InlineData("NO", "Norway")]
    [InlineData("FI", "Finland")]
    [InlineData("DK", "Denmark")]
    [InlineData("LT", "Lithuania")]
    [InlineData("LV", "Latvia")]
    [InlineData("EE", "Estonia")]
    public void Create_Country(string code, string name)
    {
        var country = new Country(code);
        Assert.Equal(code, country.Code);
        Assert.Equal(name, country.Name);
    }

    [Fact]
    public void Fail_To_Create_Country_Missing_Code()
    {
        const string code = "";
        var ex = Assert.Throws<TextValidationException>(() => new Country(code));

        Assert.Equal(code, ex.AttemptedValue);
        Assert.Equal("code", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.Required, ex.ErrorCode);
        Assert.Equal("Country code is required", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_Country_Wrong_Code()
    {
        const string code = "XY";
        var ex = Assert.Throws<DomainLogicException>(() => new Country(code));

        Assert.Equal("code", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.IncorrectValue, ex.ErrorCode);
        Assert.Equal($"Country code value {code} is not allowed", ex.Message);
    }
}