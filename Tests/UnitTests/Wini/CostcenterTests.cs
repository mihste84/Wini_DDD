namespace Tests.UnitTests.Wini;

public class CostcenterTests
{
    [Fact]
    public void Create_Costcenter()
    {
        var costcenter = new Costcenter("N0000");
        Assert.Equal("N0000", costcenter.Code);

        costcenter = new Costcenter(default);
        Assert.Equal(default, costcenter.Code);

        costcenter = new Costcenter();
        Assert.Equal(default, costcenter.Code);
    }

    [Theory]
    [InlineData("N00000")]
    public void Fail_To_Create_Costcenter_With_Invalid_Code(string? code)
    {
        var ex = Assert.Throws<DomainValidationException>(() => new Costcenter(code));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(code, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("The length of 'Costcenter' must be 5 characters or fewer. You entered 6 characters.", error.Message);
    }
}