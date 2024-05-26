namespace AppLogic.WiniLogic.Models;

public record SearchResultWrapper<M>(
    bool HasMorePages,
    M[] Items,
    int NextStartRow,
    int NextEndRow
);