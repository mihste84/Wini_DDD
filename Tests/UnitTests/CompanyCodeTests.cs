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
        var ex = Assert.Throws<TextValidationException>(() => new CompanyCode(companyCode));

        Assert.Equal(companyCode, ex.AttemptedValue);
        Assert.Equal("companyCodeString", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.IncorrectFormat, ex.ErrorCode);
        Assert.Equal("CompanyCodeString could not be parsed to a numeric value", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_Company_With_Invalid_String_Company_Code()
    {
        const string companyCode = "1000";
        var ex = Assert.Throws<NumberValidationException>(() => new CompanyCode(companyCode));

        Assert.Equal(1000, ex.AttemptedValue);
        Assert.Equal("companyCode", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.OutOfRange, ex.ErrorCode);
        Assert.Equal("CompanyCode must be between 0 and 999", ex.Message);
        Assert.Equal(0, ex.MinValue);
        Assert.Equal(999, ex.MaxValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    public void Fail_To_Create_Company_With_Invalid_Company_Code(int? companyCode)
    {
        var ex = Assert.Throws<NumberValidationException>(() => new CompanyCode(companyCode));

        Assert.Equal(companyCode, ex.AttemptedValue);
        Assert.Equal("companyCode", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.OutOfRange, ex.ErrorCode);
        Assert.Equal("CompanyCode must be between 0 and 999", ex.Message);
        Assert.Equal(0, ex.MinValue);
        Assert.Equal(999, ex.MaxValue);
    }
}