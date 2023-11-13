namespace Tests.UnitTests.Wini;

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
        var ex = Assert.Throws<DomainValidationException>(() => new Country(code));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(code, error.AttemptedValue);
        Assert.Equal("IncorrectValue", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("Country code value is not allowed.", error.Message);
    }
}