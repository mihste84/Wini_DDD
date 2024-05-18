using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.Primitives;

namespace API.Models;

public record struct BookingSearchInput
{
    public SearchItem[] SearchItems { get; set; } = [];
    public int StartRow { get; set; } = 0;
    public int EndRow { get; set; } = 10;
    public string OrderBy { get; set; } = "Id";
    public string OrderByDirection { get; set; } = "DESC";

    public BookingSearchInput() { }
    public BookingSearchInput(Dictionary<string, StringValues>? query)
    {
        if (query?.Count > 0)
        {
            _ = int.TryParse(query["startRow"], out var startRow);
            _ = int.TryParse(query["endRow"], out var endRow);
            _ = query.TryGetValue("orderBy", out var orderBy);
            _ = query.TryGetValue("orderByDirection", out var orderByDirection);
            StartRow = startRow;
            EndRow = endRow;
            OrderBy = orderBy.ToString();
            OrderByDirection = orderByDirection.ToString();
            SearchItems = query
                .Where(_ => _.Key.StartsWith("searchitems", StringComparison.CurrentCultureIgnoreCase))
                .GroupBy(_ => _.Key.Split('.').First())
                .Select(_ => new SearchItem(
                    GetKeyPairValue(_, ".name") ?? "",
                    GetKeyPairValue(_, ".value") ?? "",
                    GetKeyPairValue(_, ".operator") ?? "",
                    bool.Parse(GetKeyPairValue(_, ".handleAutomatically") ?? "true")
                )).ToArray();
        }
    }

    public static ValueTask<BookingSearchInput?> BindAsync(HttpContext context, ParameterInfo _)
    {
        var dict = context.Request.Query.ToDictionary();

        var input = new BookingSearchInput(dict);

        return ValueTask.FromResult<BookingSearchInput?>(input);
    }

    public static string? GetKeyPairValue(IGrouping<string, KeyValuePair<string, StringValues>> searchItem, string propertyName)
    {
        var value = searchItem.FirstOrDefault(x => x.Key.EndsWith(propertyName, StringComparison.CurrentCultureIgnoreCase));
        return value.Value;
    }
}