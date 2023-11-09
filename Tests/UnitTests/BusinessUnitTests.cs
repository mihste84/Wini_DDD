namespace Tests.UnitTests;

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
        Assert.Equal("", bu.Product.Code);

        bu = new BusinessUnit("   100      ");
        Assert.Equal(100, bu.CompanyCode.Code);
        Assert.Equal("", bu.Costcenter.Code);
        Assert.Equal("", bu.Product.Code);
    }

    [Fact]
    public void Fail_To_Create_BusinessUnit_Too_Long_Value()
    {
        const string bu = "100NKTOT12345";
        var ex = Assert.Throws<TextValidationException>(() => new BusinessUnit(bu));

        Assert.Equal(bu, ex.AttemptedValue);
        Assert.Equal("businessUnit", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("BusinessUnit cannot be longer than 12 characters", ex.Message);
        Assert.Equal(12, ex.MaxLength);
    }

    [Theory]
    [InlineData("XYZNKTOT1234", typeof(TextValidationException), "CompanyCodeString could not be parsed to a numeric value")]
    [InlineData("1NKTOT1234", typeof(TextValidationException), "CompanyCodeString could not be parsed to a numeric value")]
    public void Fail_To_Create_Business_Unit_Wrong_Values(string bu, Type typeOfException, string errorMessage)
    {
        var ex = Assert.Throws(typeOfException, () => new BusinessUnit(bu));
        Assert.Equal(errorMessage, ex.Message);
    }
}