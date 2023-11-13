namespace Tests.UnitTests.Wini;

public class AuthorizerTests
{
    [Fact]
    public void Create_Authorizer()
    {
        var cms = new Authorizer("MIHSTE", true);
        Assert.Equal("MIHSTE", cms.UserId);
        Assert.True(cms.HasAuthorized);

        cms = new Authorizer("", false);
        Assert.Equal("", cms.UserId);
        Assert.False(cms.HasAuthorized);

        cms = new Authorizer();
        Assert.Equal(default, cms.UserId);
        Assert.False(cms.HasAuthorized);
    }

    [Fact]
    public void Fail_To_Create_Authorizer_With_Empty_Value_When_Authorized()
    {
        const string authorizer = "";
        const bool hasAuthorized = true;
        var ex = Assert.Throws<DomainValidationException>(() => new Authorizer(authorizer, hasAuthorized));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
        Assert.Equal(authorizer, error.AttemptedValue);
        Assert.Equal("UserId", error.PropertyName);
        Assert.Equal("Authorizer needs to be set when row is authorized.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_Authorizer_With_Too_Long_Value()
    {
        const string authorizer = "XYZASDFGH";
        const bool hasAuthorized = true;
        var ex = Assert.Throws<DomainValidationException>(() => new Authorizer(authorizer, hasAuthorized));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal(authorizer, error.AttemptedValue);
        Assert.Equal("UserId", error.PropertyName);
        Assert.Equal("The length of 'Authorizer' must be 8 characters or fewer. You entered 9 characters.", error.Message);
    }
}