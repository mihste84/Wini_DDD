namespace Domain.Lighthouse.Values;

public abstract record BaseBookingRow
{
    public readonly int RowNumber;
    public Costcenter Costcenter { get; }
    public Product Product { get; }
    public Account Account { get; }
    public Subledger Subledger { get; }
    public Money Money { get; }

    public BaseBookingRow(
        int rowNumber,
        Costcenter costcenter,
        Product product,
        Account account,
        Subledger subledger,
        Money amount)
    {
        RowNumber = rowNumber;
        Costcenter = costcenter;
        Product = product;
        Account = account;
        Subledger = subledger;
        Money = amount;
    }

    public BaseBookingRow(int rowNumber)
    {
        RowNumber = rowNumber;
        Costcenter = new Costcenter();
        Product = new Product();
        Account = new Account();
        Subledger = new Subledger();
        Money = new Money();
    }
}