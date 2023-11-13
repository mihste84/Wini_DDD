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

        CostObject = new CostObject(3, default, "1");
        Assert.Equal(default, CostObject.Value);
        Assert.Equal("1", CostObject.Type);
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

        var ex = Assert.Throws<DomainValidationException>(() => new CostObject(number, costObject, type));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(number, error.AttemptedValue);
        Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
        Assert.Equal("Number", error.PropertyName);
        Assert.Equal("'Cost Object Number' must be between 1 and 4. You entered 5.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Too_Small_Number()
    {
        const string costObject = "123456789";
        const string type = "1";
        const int number = 0;

        var ex = Assert.Throws<DomainValidationException>(() => new CostObject(number, costObject, type));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(number, error.AttemptedValue);
        Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
        Assert.Equal("Number", error.PropertyName);
        Assert.Equal("'Cost Object Number' must be between 1 and 4. You entered 0.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Too_Many_Chars()
    {
        const string costObject = "123456789101112";
        const string type = "1";
        const int number = 1;
        var ex = Assert.Throws<DomainValidationException>(() => new CostObject(number, costObject, type));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(costObject, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Value", error.PropertyName);
        Assert.Equal("The length of 'Cost Object 1' must be 12 characters or fewer. You entered 15 characters.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_CostObject_With_Type_With_Too_Many_Chars()
    {
        const string costObject = "12345";
        const string type = "1234567";
        const int number = 1;
        var ex = Assert.Throws<DomainValidationException>(() => new CostObject(number, costObject, type));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(type, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Type", error.PropertyName);
        Assert.Equal("The length of 'Cost Object Type 1' must be 1 characters or fewer. You entered 7 characters.", error.Message);
    }
}