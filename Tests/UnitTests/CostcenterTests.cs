namespace Tests.UnitTests;

public class CostcenterTests
{
    [Fact]
    public void Create_Costcenter()
    {
        var costcenter = new Costcenter("N0000");
        Assert.Equal("N0000", costcenter.Code);

        costcenter = new Costcenter(default);
        Assert.Equal(default, costcenter.Code);
    }

    [Theory]
    [InlineData("N00000")]
    public void Fail_To_Create_Costcenter_With_Invalid_Code(string? code)
    {
        var ex = Assert.Throws<TextValidationException>(() => new Costcenter(code));

        Assert.Equal(code, ex.AttemptedValue);
        Assert.Equal("costcenter", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Costcenter cannot be longer than 5 characters", ex.Message);
        Assert.Equal(5, ex.MaxLength);
    }
}