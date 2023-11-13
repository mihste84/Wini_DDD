namespace Tests.UnitTests;

public class CompanyCodeTests
{
    [Fact]
    public void Create_Company()
    {
        var company = new CompanyCode(100);
        Assert.Equal(100, company.Code);

        company = new CompanyCode((int?)default);
        Assert.Equal(default, company.Code);
    }

    [Fact]
    public void Create_Company_With_String()
    {
        var company = new CompanyCode("100");
        Assert.Equal(100, company.Code);

        company = new CompanyCode("   ");
        Assert.Equal(default, company.Code);
    }

    [Fact]
    public void Fail_To_Create_Company_With_Non_Numeric_String()
    {
        const string companyCode = "XYZ";
        var ex = Assert.Throws<FormatException>(() => new CompanyCode(companyCode));

        Assert.Equal("CompanyCodeString could not be parsed to a numeric value", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_Company_With_Invalid_String_Company_Code()
    {
        const string companyCode = "1000";
        var ex = Assert.Throws<DomainValidationException>(() => new CompanyCode(companyCode));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(1000, error.AttemptedValue);
        Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("'Company Code' must be between 0 and 999. You entered 1000.", error.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    public void Fail_To_Create_Company_With_Invalid_Company_Code(int? companyCode)
    {
        var ex = Assert.Throws<DomainValidationException>(() => new CompanyCode(companyCode));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(companyCode, error.AttemptedValue);
        Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal($"'Company Code' must be between 0 and 999. You entered {companyCode}.", error.Message);
    }
}