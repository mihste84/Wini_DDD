namespace Tests.UnitTests;

public class FileContentTests
{
    [Fact]
    public void Create_FileContent()
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        const string path = "path/to/file";
        var file = new FileContent(1000, contentType, name, path);
        Assert.Equal(1000, file.Size);
        Assert.Equal(name, file.Name);
        Assert.Equal(path, file.Path);
        Assert.Equal(contentType, file.ContentType);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Wrong_Content_Type()
    {
        const string contentType = "application/json";
        const string name = "test.txt";
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<DomainLogicException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("contentType", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.IncorrectValue, ex.ErrorCode);
        Assert.Equal("ContentType value is not allowed", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Empty_Content_Type()
    {
        const string contentType = "";
        const string name = "test.txt";
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<TextValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("contentType", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.Required, ex.ErrorCode);
        Assert.Equal("ContentType cannot be empty", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10000000)]
    public void Fail_To_Create_FileContent_Size_Out_Of_Range(int size)
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        const string path = "path/to/file";

        var ex = Assert.Throws<NumberValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("size", ex.PropertyName);
        Assert.Equal(size, ex.AttemptedValue);
        Assert.Equal(ValidationErrorCodes.OutOfRange, ex.ErrorCode);
        Assert.Equal("Size must be between 0 and 2000000 byte", ex.Message);
        Assert.Equal(2000000, ex.MaxValue);
        Assert.Equal(0, ex.MinValue);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Empty_Path()
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        const string path = "";
        const int size = 1000;

        var ex = Assert.Throws<TextValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("path", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.Required, ex.ErrorCode);
        Assert.Equal("Path cannot be empty", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Path_Too_Long()
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        var path = new string('a', 240);
        const int size = 1000;

        var ex = Assert.Throws<TextValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("path", ex.PropertyName);
        Assert.Equal(path, ex.AttemptedValue);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Path cannot be longer than 200 characters", ex.Message);
        Assert.Equal(200, ex.MaxLength);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Empty_Name()
    {
        const string contentType = "text/plain";
        const string name = "";
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<TextValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("name", ex.PropertyName);
        Assert.Equal(ValidationErrorCodes.Required, ex.ErrorCode);
        Assert.Equal("Name cannot be empty", ex.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Name_Too_Long()
    {
        const string contentType = "text/plain";
        var name = new string('a', 240);
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<TextValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Equal("name", ex.PropertyName);
        Assert.Equal(name, ex.AttemptedValue);
        Assert.Equal(ValidationErrorCodes.TextTooLong, ex.ErrorCode);
        Assert.Equal("Name cannot be longer than 100 characters", ex.Message);
        Assert.Equal(100, ex.MaxLength);
    }
}