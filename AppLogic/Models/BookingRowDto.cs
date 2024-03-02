namespace AppLogic.Models;

public record BookingRowDto(
    long RowNumber,
    string? BusinessUnit,
    string? Account,
    string? Subsidiary,
    string? Subledger,
    string? SubledgerType,
    string? CostObject1,
    string? CostObjectType1,
    string? CostObject2,
    string? CostObjectType2,
    string? CostObject3,
    string? CostObjectType3,
    string? CostObject4,
    string? CostObjectType4,
    string? Remark,
    string? Authorizer,
    bool IsAuthorized,
    decimal? Amount,
    string? CurrencyCode,
    decimal? ExchangeRate
);