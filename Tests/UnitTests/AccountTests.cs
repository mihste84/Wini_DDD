namespace Tests.UnitTests;

public class AccountTests
{
    [Fact]
    public void Create_Account()
    {
        var account = new Account("12345");
        Assert.Equal("12345", account.Value);
        Assert.Equal(default, account.Subsidiary);

        account = new Account("12345", "1234");
        Assert.Equal("12345", account.Value);
        Assert.Equal("1234", account.Subsidiary);

        account = new Account(default);
        Assert.Equal(default, account.Value);
        Assert.Equal(default, account.Subsidiary);
    }

    [Fact]
    public void Fail_To_Create_Account_With_Too_Many_Chars()
    {
        const string account = "1234567";
        var ex = Assert.Throws<TextValidationException>(() => new Account(account, "1234"));

        Assert.Equal(account, ex.AttemptedValue);
        Assert.Equal("account", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Account cannot be longer than 6 characters", ex.Message);
        Assert.Equal(6, ex.MaxLength);
    }

    [Fact]
    public void Fail_To_Create_Account_With_Subsidiary_With_Too_Many_Chars()
    {
        const string account = "12345";
        const string subsidiary = "123456789";
        var ex = Assert.Throws<TextValidationException>(() => new Account(account, subsidiary));

        Assert.Equal(subsidiary, ex.AttemptedValue);
        Assert.Equal("subsidiary", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Subsidiary cannot be longer than 8 characters", ex.Message);
        Assert.Equal(8, ex.MaxLength);
    }
}