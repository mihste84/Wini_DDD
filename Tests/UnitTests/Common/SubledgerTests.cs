namespace Tests.UnitTests.Common;

public class SubledgerTests
{
    [Fact]
    public void Create_Subledger()
    {
        var subledger = new Subledger("12345", default);
        Assert.Equal("12345", subledger.Value);
        Assert.Equal(default, subledger.Type);

        subledger = new Subledger("12345", "A");
        Assert.Equal("12345", subledger.Value);
        Assert.Equal("A", subledger.Type);

        subledger = new Subledger(default, default);
        Assert.Equal(default, subledger.Value);
        Assert.Equal(default, subledger.Type);

        subledger = new Subledger();
        Assert.Equal(default, subledger.Value);
        Assert.Equal(default, subledger.Type);
    }

    [Fact]
    public void Fail_To_Create_Subledger_With_Too_Many_Chars()
    {
        const string subledger = "123456789";
        var ex = Assert.Throws<DomainValidationException>(() => new Subledger(subledger, default));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(subledger, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Value", error.PropertyName);
        Assert.Equal("The length of 'Subledger' must be 8 characters or fewer. You entered 9 characters.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_Subledger_With_Type_With_Too_Many_Chars()
    {
        const string subledger = "12345";
        const string type = "ABC";
        var ex = Assert.Throws<DomainValidationException>(() => new Subledger(subledger, type));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(type, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Type", error.PropertyName);
        Assert.Equal("The length of 'Subledger Type' must be 1 characters or fewer. You entered 3 characters.", error.Message);
    }
}