namespace Domain.Wini.Models;

public record BookingRowModel
{
    public int RowNumber { get; set; }
    public string? BusinessUnit { get; set; }
    public string? Account { get; set; }
    public string? Subsidiary { get; set; }
    public string? Subledger { get; set; }
    public string? SubledgerType { get; set; }
    public string? CostObject1 { get; set; }
    public string? CostObjectType1 { get; set; }
    public string? CostObject2 { get; set; }
    public string? CostObjectType2 { get; set; }
    public string? CostObject3 { get; set; }
    public string? CostObjectType3 { get; set; }
    public string? CostObject4 { get; set; }
    public string? CostObjectType4 { get; set; }
    public string? Remark { get; set; }
    public string? Authorizer { get; set; }
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? CurrencyRate { get; set; }
}