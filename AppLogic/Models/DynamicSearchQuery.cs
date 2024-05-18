namespace AppLogic.Models;

public record SearchItem(string Name, string? Value, string Operator, bool HandleAutomatically = true);

public static class SearchOperators
{
    public const string Equal = "Equal";
    public const string NotEqual = "NotEqual";
    public const string GreaterThan = "GreaterThan";
    public const string GreaterThanOrEqual = "GreaterThanOrEqual";
    public const string LessThan = "LessThan";
    public const string LessThanOrEqual = "LessThanOrEqual";
    public const string Contains = "Contains";
    public const string StartsWith = "StartsWith";
    public const string EndsWith = "EndsWith";
    public const string IsNull = "IsNull";
    public const string IsNotNull = "IsNotNull";
    public const string In = "In";
    public const string NotIn = "NotIn";
}

public class DynamicSearchQuery
{
    public SearchItem[] SearchItems { get; set; } = [];
    public int StartRow { get; set; } = 0;
    public int EndRow { get; set; } = 10;
    public string OrderBy { get; set; } = "Id";
    public string OrderByDirection { get; set; } = "ASC";

    public static FormattableString GetWhereFromSearchItem(SearchItem item)
    => item.Operator switch
    {
        SearchOperators.Equal => $"{item.Name:raw} = {item.Value}",
        SearchOperators.NotEqual => $"{item.Name:raw} <> {item.Value}",
        SearchOperators.GreaterThan => $"{item.Name:raw} > {item.Value}",
        SearchOperators.GreaterThanOrEqual => $"{item.Name:raw} >= {item.Value}",
        SearchOperators.LessThan => $"{item.Name:raw} < {item.Value}",
        SearchOperators.LessThanOrEqual => $"{item.Name:raw} <= {item.Value}",
        SearchOperators.StartsWith => $"{item.Name:raw} LIKE '{item.Value + "%"}'",
        SearchOperators.EndsWith => $"{item.Name:raw} LIKE '{"%" + item.Value}'",
        SearchOperators.Contains => $"{item.Name:raw} LIKE '{"%" + item.Value + "%"}'",
        SearchOperators.In => $"{item.Name:raw} IN ({item.Value?.Split(',')})",
        SearchOperators.NotIn => $"{item.Name:raw} NOT IN ({item.Value?.Split(',')})",
        SearchOperators.IsNull => $"({item.Name:raw} IS NULL OR {item.Name:raw} = '')",
        SearchOperators.IsNotNull => $"{item.Name:raw} IS NOT NULL AND {item.Name:raw} <> ''",
        _ => throw new ArgumentOutOfRangeException($"Operator '{item.Operator}' not supported.")
    };

    public bool TryExtractSearchItemByName(string name, out SearchItem? item)
    {
        item = SearchItems.FirstOrDefault(_ => _.Name == name);
        return item != null;
    }
}