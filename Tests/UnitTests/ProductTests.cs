namespace Tests.UnitTests;

public class ProductTests
{
    [Fact]
    public void Create_Product()
    {
        var product = new Product("1234");
        Assert.Equal("1234", product.Code);

        product = new Product(default);
        Assert.Equal(default, product.Code);
    }

    [Theory]
    [InlineData("123456")]
    public void Fail_To_Create_Product_With_Invalid_Code(string? code)
    {
        var ex = Assert.Throws<TextValidationException>(() => new Product(code));

        Assert.Equal(code, ex.AttemptedValue);
        Assert.Equal("product", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Product cannot be longer than 4 characters", ex.Message);
        Assert.Equal(4, ex.MaxLength);
    }
}