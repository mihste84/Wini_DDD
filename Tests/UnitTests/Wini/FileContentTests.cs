namespace Tests.UnitTests.Wini;

public class FileContentTests
{
    [Theory]
    [InlineData("text/plain")]
    [InlineData("application/pdf")]
    [InlineData("application/vnd.ms-excel")]
    [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("application/msword")]
    [InlineData("image/png")]
    [InlineData("image/jpeg")]
    [InlineData("application/octet-stream")]
    public void Create_Attachment(string contentType)
    {
        const int size = 1024;
        const string filename = "Test.csv";
        const string path = "path/to/file";

        var fc = new FileContent(size, contentType, filename, path);

        Assert.Equal(size, fc.Size);
        Assert.Equal(filename, fc.Name);
        Assert.Equal(path, fc.Path);
        Assert.Equal(contentType, fc.ContentType);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Wrong_Content_Type()
    {
        const string contentType = "application/json";
        const string name = "test.txt";
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(contentType, error.AttemptedValue);
        Assert.Equal("PredicateValidator", error.ErrorCode);
        Assert.Equal("ContentType", error.PropertyName);
        Assert.Equal($"Content type value '{contentType}' is not allowed.", error.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10000000)]
    public void Fail_To_Create_FileContent_Size_Out_Of_Range(long size)
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        const string path = "path/to/file";

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(size, error.AttemptedValue);
        Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
        Assert.Equal("Size", error.PropertyName);
        Assert.Equal($"Size must be between 0 and {FileContent.MaxAttachmentSize} byte.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Empty_Path()
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        const string path = "";
        const int size = 5000000;

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(path, error.AttemptedValue);
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
        Assert.Equal("Path", error.PropertyName);
        Assert.Equal("'File Path' must not be empty.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Path_Too_Long()
    {
        const string contentType = "text/plain";
        const string name = "test.txt";
        var path = new string('a', 500);
        const int size = 1000;

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(path, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Path", error.PropertyName);
        Assert.Equal("The length of 'File Path' must be 400 characters or fewer. You entered 500 characters.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Empty_Name()
    {
        const string contentType = "text/plain";
        const string name = "";
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(name, error.AttemptedValue);
        Assert.Equal("NotEmptyValidator", error.ErrorCode);
        Assert.Equal("Name", error.PropertyName);
        Assert.Equal("'Filename' must not be empty.", error.Message);
    }

    [Fact]
    public void Fail_To_Create_FileContent_Name_Too_Long()
    {
        const string contentType = "text/plain";
        var name = new string('a', 320);
        const string path = "path/to/file";
        const int size = 1000;

        var ex = Assert.Throws<DomainValidationException>(() => new FileContent(size, contentType, name, path));

        Assert.Single(ex.Errors);
        var error = ex.Errors.First();
        Assert.Equal(name, error.AttemptedValue);
        Assert.Equal("MaximumLengthValidator", error.ErrorCode);
        Assert.Equal("Name", error.PropertyName);
        Assert.Equal("The length of 'Filename' must be 300 characters or fewer. You entered 320 characters.", error.Message);
    }
}