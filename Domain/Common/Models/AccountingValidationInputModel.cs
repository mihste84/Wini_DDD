namespace Domain.Common.Models;

public record AccountingValidationInputModel
{
    public string? BusinessUnit { get; set; }
    public string? Account { get; set; }
    public string? Subsidiary { get; set; }
    public string? Subledger { get; set; }
    public string? CostObject1 { get; set; }
    public string? CostObject2 { get; set; }
    public string? CostObject3 { get; set; }
    public string? CostObject4 { get; set; }
    public string? CostObject1Type { get; set; }
    public string? CostObject2Type { get; set; }
    public string? CostObject3Type { get; set; }
    public string? CostObject4Type { get; set; }
    public string? Currency { get; set; }
    public long? BookingRow { get; set; }
}