namespace Tests.UnitTests;

public class SubledgerTests
{
    [Fact]
    public void Create_Subledger()
    {
        var Subledger = new Subledger("12345", default);
        Assert.Equal("12345", Subledger.Value);
        Assert.Equal(default, Subledger.Type);

        Subledger = new Subledger("12345", "A");
        Assert.Equal("12345", Subledger.Value);
        Assert.Equal("A", Subledger.Type);

        Subledger = new Subledger(default, default);
        Assert.Equal(default, Subledger.Value);
        Assert.Equal(default, Subledger.Type);
    }

    [Fact]
    public void Fail_To_Create_Subledger_With_Too_Many_Chars()
    {
        const string subledger = "123456789";
        var ex = Assert.Throws<TextValidationException>(() => new Subledger(subledger, default));

        Assert.Equal(subledger, ex.AttemptedValue);
        Assert.Equal("subledger", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Subledger cannot be longer than 8 characters", ex.Message);
        Assert.Equal(8, ex.MaxLength);
    }

    [Fact]
    public void Fail_To_Create_Subledger_With_Type_With_Too_Many_Chars()
    {
        const string subledger = "12345";
        const string type = "ABC";
        var ex = Assert.Throws<TextValidationException>(() => new Subledger(subledger, type));

        Assert.Equal(type, ex.AttemptedValue);
        Assert.Equal("type", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Subledger type cannot be longer than 1 characters", ex.Message);
        Assert.Equal(1, ex.MaxLength);
    }
}