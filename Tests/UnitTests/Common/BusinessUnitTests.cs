namespace Tests.UnitTests.Common;

public class BusinessUnitTests
{
    [Fact]
    public void Create_BusinessUnit()
    {
        var bu = new BusinessUnit("100NKTOT1234");
        Assert.Equal(100, bu.CompanyCode.Code);
        Assert.Equal("NKTOT", bu.Costcenter.Code);
        Assert.Equal("1234", bu.Product.Code);

        bu = new BusinessUnit("100NKTOT");
        Assert.Equal(100, bu.CompanyCode.Code);
        Assert.Equal("NKTOT", bu.Costcenter.Code);
        Assert.Equal(default, bu.Product.Code);

        bu = new BusinessUnit("   100      ");
        Assert.Equal(100, bu.CompanyCode.Code);
        Assert.Equal(default, bu.Costcenter.Code);
        Assert.Equal(default, bu.Product.Code);

        bu = new BusinessUnit("");
        Assert.Equal(default, bu.CompanyCode.Code);
        Assert.Equal(default, bu.Costcenter.Code);
        Assert.Equal(default, bu.Product.Code);

        bu = new BusinessUnit();
        Assert.Equal(default, bu.CompanyCode.Code);
        Assert.Equal(default, bu.Costcenter.Code);
        Assert.Equal(default, bu.Product.Code);
    }

    [Fact]
    public void Fail_To_Create_BusinessUnit_Too_Long_Value()
    {
        const string bu = "100NKTOT12345";
        var ex = Assert.Throws<DomainValidationException>(() => new BusinessUnit(bu));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("12345", error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("The length of 'Product' must be 4 characters or fewer. You entered 5 characters.", error.Message);
    }

    [Theory]
    [InlineData("XYZNKTOT1234", "CompanyCodeString could not be parsed to a numeric value")]
    [InlineData("1NKTOT1234", "CompanyCodeString could not be parsed to a numeric value")]
    public void Fail_To_Create_Business_Unit_Wrong_Values(string bu, string errorMessage)
    {
        var ex = Assert.Throws<FormatException>(() => new BusinessUnit(bu));
        Assert.Equal(errorMessage, ex.Message);
    }
}