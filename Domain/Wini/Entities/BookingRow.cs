namespace Domain.Wini.Entities;

public class BookingRow
{
    public readonly IdValue<int>? Id;
    public readonly IdValue<int>? BookingId;
    public BusinessUnit BusinessUnit { get; set; }
    public Account Account { get; set; }
    public Subledger Subledger { get; set; }
    public CostObject CostObject1 { get; set; }
    public CostObject CostObject2 { get; set; }
    public CostObject CostObject3 { get; set; }
    public CostObject CostObject4 { get; set; }
    public Remark Remark { get; set; }
    public Authorizer Authorizer { get; set; }
    public Money Amount { get; set; }

    public BookingRow(
        IdValue<int>? id,
        IdValue<int>? bookingId,
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
        Id = id;
        BookingId = bookingId;
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

    public BookingRow(
        IdValue<int>? id,
        IdValue<int>? bookingId)
    {
        Id = id;
        BookingId = bookingId;
        BusinessUnit = new BusinessUnit();
        Account = new Account();
        Subledger = new Subledger();
        CostObject1 = new CostObject(1);
        CostObject2 = new CostObject(2);
        CostObject3 = new CostObject(3);
        CostObject4 = new CostObject(4);
        Remark = new Remark();
        Authorizer = new Authorizer();
        Amount = new Money();
    }
}