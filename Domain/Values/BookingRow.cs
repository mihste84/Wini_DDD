namespace Domain.Values;

public class BookingRow
{
    public readonly IdValue<int> BookingId;
    public readonly BookingRowNumber RowNumber;
    public BusinessUnit BusinessUnit { get; }
    public Account Account { get; }
    public Subledger Subledger { get; }
    public CostObject CostObject1 { get; }
    public CostObject CostObject2 { get; }
    public CostObject CostObject3 { get; }
    public CostObject CostObject4 { get; }
    public Remark Remark { get; }
    public Authorizer Authorizer { get; }
    public Money Amount { get; }

    public BookingRow(
        IdValue<int> bookingId,
        BookingRowNumber rowNumber,
        BusinessUnit bu,
        Account account,
        Subledger subledger,
        CostObject costObject1,
        CostObject costObject2,
        CostObject costObject3,
        CostObject costObject4,
        Remark remark,
        Authorizer authorizer,
        Money amount)
    {
        BookingId = bookingId;
        RowNumber = rowNumber;
        BusinessUnit = bu;
        Account = account;
        Subledger = subledger;
        CostObject1 = costObject1;
        CostObject2 = costObject2;
        CostObject3 = costObject3;
        CostObject4 = costObject4;
        Remark = remark;
        Authorizer = authorizer;
        Amount = amount;
    }
}