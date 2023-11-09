namespace Tests.UnitTests;

public class IdValueTests
{
    [Fact]
    public void Create_IdValue()
    {
        var IdValue = new IdValue<int>(1);
        Assert.Equal(1, IdValue.Value);
    }
}