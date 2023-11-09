namespace Tests.UnitTests;

public class CostObjectTests
{
    [Fact]
    public void Create_CostObject()
    {
        var CostObject = new CostObject(1, "12345", default);
        Assert.Equal("12345", CostObject.Value);
        Assert.Equal(default, CostObject.Type);
        Assert.Equal(1, CostObject.Number);

        CostObject = new CostObject(2, "12345", "1");
        Assert.Equal("12345", CostObject.Value);
        Assert.Equal("1", CostObject.Type);
        Assert.Equal(2, CostObject.Number);

        CostObject = new CostObject(3, default, default);
        Assert.Equal(default, CostObject.Value);
        Assert.Equal(default, CostObject.Type);
        Assert.Equal(3, CostObject.Number);

        CostObject = new CostObject(4, default, default);
        Assert.Equal(default, CostObject.Value);
        Assert.Equal(default, CostObject.Type);
        Assert.Equal(4, CostObject.Number);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Too_Big_Number()
    {
        const string costObject = "123456789";
        const string type = "1";
        const int number = 5;
        var ex = Assert.Throws<NumberValidationException>(() => new CostObject(number, costObject, type));

        Assert.Equal(number, ex.AttemptedValue);
        Assert.Equal("number", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.OutOfRange, ex.ErrorCode);
        Assert.Equal("Cost object number must be between 1 and 4", ex.Message);
        Assert.Equal(4, ex.MaxValue);
        Assert.Equal(1, ex.MinValue);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Too_Small_Number()
    {
        const string costObject = "123456789";
        const string type = "1";
        const int number = 0;
        var ex = Assert.Throws<NumberValidationException>(() => new CostObject(number, costObject, type));

        Assert.Equal(number, ex.AttemptedValue);
        Assert.Equal("number", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.OutOfRange, ex.ErrorCode);
        Assert.Equal("Cost object number must be between 1 and 4", ex.Message);
        Assert.Equal(4, ex.MaxValue);
        Assert.Equal(1, ex.MinValue);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Too_Many_Chars()
    {
        const string CostObject = "123456789101112";
        var ex = Assert.Throws<TextValidationException>(() => new CostObject(1, CostObject, "1234"));

        Assert.Equal(CostObject, ex.AttemptedValue);
        Assert.Equal("costObject", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Cost object cannot be longer than 12 characters", ex.Message);
        Assert.Equal(12, ex.MaxLength);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Type_With_Too_Many_Chars()
    {
        const string CostObject = "12345";
        const string type = "1234567";
        var ex = Assert.Throws<TextValidationException>(() => new CostObject(1, CostObject, type));

        Assert.Equal(type, ex.AttemptedValue);
        Assert.Equal("type", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Cost object type cannot be longer than 1 character", ex.Message);
        Assert.Equal(1, ex.MaxLength);
    }
}