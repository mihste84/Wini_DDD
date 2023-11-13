namespace Tests.UnitTests.Wini;

public class ProductTests
{
    [Fact]
    public void Create_Product()
    {
        var product = new Product("1234");
        Assert.Equal("1234", product.Code);

        product = new Product(default);
        Assert.Equal(default, product.Code);

        product = new Product();
        Assert.Equal(default, product.Code);
    }

    [Theory]
    [InlineData("123456")]
    public void Fail_To_Create_Product_With_Invalid_Code(string? code)
    {
        var ex = Assert.Throws<DomainValidationException>(() => new Product(code));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(code, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Code", error.PropertyName);
        Assert.Equal("The length of 'Product' must be 4 characters or fewer. You entered 6 characters.", error.Message);
    }
}