namespace Services.DatabaseCommon.Models;

public class BookingRow
{
    public int Id { get; set; }

    public int BookingId { get; set; }

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

    public bool IsAuthorized { get; set; }

    public decimal? Amount { get; set; }

    public string? CurrencyCode { get; set; }

    public decimal? ExchangeRate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
