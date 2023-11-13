namespace Tests.UnitTests.Wini;

public class CommissionerTests
{
    [Fact]
    public void Create_Commissioner()
    {
        var cms = new Commissioner("MIHSTE");
        Assert.Equal("MIHSTE", cms.UserId);
    }

    [Fact]
    public void Fail_To_Create_Commissioner_With_Empty_Value()
    {
        const string commissioner = "";
        var ex = Assert.Throws<DomainValidationException>(() => new Commissioner(commissioner));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
        Assert.Equal(commissioner, error.AttemptedValue);
        Assert.Equal("UserId", error.PropertyName);
        Assert.Equal("'Commissioner' must not be empty.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_Commissioner_With_Too_Long_Value()
    {
        const string commissioner = "XYZASDFGH";
        var ex = Assert.Throws<DomainValidationException>(() => new Commissioner(commissioner));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal(commissioner, error.AttemptedValue);
        Assert.Equal("UserId", error.PropertyName);
        Assert.Equal("The length of 'Commissioner' must be 8 characters or fewer. You entered 9 characters.", error.Message);
    }
}