using Microsoft.Extensions.Primitives;

namespace Tests.UnitTests.Wini;

public class BookingSearchInputTests
{
    [Fact]
    public void Booking_Search_Input_Constructor()
    {
        // Arrange
        var query = new Dictionary<string, StringValues>
        {
            { "startRow", new StringValues("1") },
            { "endRow", new StringValues("2") },
            { "orderBy", new StringValues("Id") },
            { "orderByDirection", new StringValues("DESC") },
            { "searchitems[0].name", new StringValues("Name1") },
            { "searchitems[0].value", new StringValues("Value1") },
            { "searchitems[0].operator", new StringValues("Equals") },
            { "searchitems[0].handleAutomatically", new StringValues("true") },
            { "searchitems[1].name", new StringValues("Name2") },
            { "searchitems[1].value", new StringValues("Value2") },
            { "searchitems[1].operator", new StringValues("Contains") },
            { "searchitems[1].handleAutomatically", new StringValues("false") }
        };

        // Act
        var input = new BookingSearchInput(query);

        // Assert
        Assert.Equal(1, input.StartRow);
        Assert.Equal(2, input.EndRow);
        Assert.Equal("Id", input.OrderBy);
        Assert.Equal("DESC", input.OrderByDirection);
        Assert.Equal(2, input.SearchItems.Length);
        Assert.Equal("Name1", input.SearchItems[0].Name);
        Assert.Equal("Value1", input.SearchItems[0].Value);
        Assert.Equal("Equals", input.SearchItems[0].Operator);
        Assert.True(input.SearchItems[0].HandleAutomatically);
        Assert.Equal("Name2", input.SearchItems[1].Name);
        Assert.Equal("Value2", input.SearchItems[1].Value);
        Assert.Equal("Contains", input.SearchItems[1].Operator);
        Assert.False(input.SearchItems[1].HandleAutomatically);
    }
}