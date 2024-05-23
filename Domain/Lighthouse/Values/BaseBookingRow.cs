namespace Domain.Lighthouse.Values;

public abstract record BaseBookingRow
{
    public readonly int RowNumber;
    public BusinessUnit BusinessUnit { get; }
    public Account Account { get; }
    public Subledger Subledger { get; }
    public Money Money { get; }

    public BaseBookingRow(
        int rowNumber,
        BusinessUnit businessUnit,
        Account account,
        Subledger subledger,
        Money amount)
    {
        RowNumber = rowNumber;
        BusinessUnit = businessUnit;
        Account = account;
        Subledger = subledger;
        Money = amount;
    }

    public BaseBookingRow(int rowNumber)
    {
        RowNumber = rowNumber;
        BusinessUnit = new BusinessUnit();
        Account = new Account();
        Subledger = new Subledger();
        Money = new Money();
    }
}