namespace Tests.UnitTests;

public class CommissionerTests
{
    [Fact]
    public void Create_Commissioner()
    {
        var cms = new Commissioner("MIHSTE");
        Assert.Equal("MIHSTE", cms.UserId);
    }

    [Theory]
    [InlineData("", "Commissioner cannot be empty", ValidationErrorCodes.Required)]
    [InlineData(" ", "Commissioner cannot be empty", ValidationErrorCodes.Required)]
    [InlineData(null, "Commissioner cannot be empty", ValidationErrorCodes.Required)]
    [InlineData("ASDFGHJKL", "User ID cannot be longer than 8 characters", ValidationErrorCodes.TextTooLong)]
    public void Fail_To_Create_Commissioner_With_Invalid_Value(string Commissioner, string errorMessage, int errorCode)
    {
        var ex = Assert.Throws<TextValidationException>(() => new Commissioner(Commissioner));

        Assert.Contains(errorMessage, ex.Message);
        Assert.Equal(errorCode, ex.ErrorCode);
    }
}