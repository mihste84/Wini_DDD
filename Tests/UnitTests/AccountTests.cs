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
        var ex = Assert.Throws<DomainValidationException>(() => new Account(account, "1234"));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal(account, error.AttemptedValue);
        Assert.Equal("Value", error.PropertyName);
        Assert.Equal("The length of 'Account' must be 6 characters or fewer. You entered 7 characters.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_Account_With_Subsidiary_With_Too_Many_Chars()
    {
        const string account = "";
        const string subsidiary = "123456789";
        var ex = Assert.Throws<DomainValidationException>(() => new Account(account, subsidiary));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal(subsidiary, error.AttemptedValue);
        Assert.Equal("Subsidiary", error.PropertyName);
        Assert.Equal("The length of 'Subsidiary' must be 8 characters or fewer. You entered 9 characters.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_Account_With_Account_And_Subsidiary_With_Too_Many_Chars()
    {
        const string account = "1234567";
        const string subsidiary = "123456789";
        var ex = Assert.Throws<DomainValidationException>(() => new Account(account, subsidiary));

        Assert.Equal(2, ex.Errors.Count());
    }
}