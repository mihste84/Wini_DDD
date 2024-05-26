namespace Domain.Lighthouse.Values;

public record PolicyBookingRow
{
    public readonly int RowNumber;
    public readonly Policy Policy;
    public readonly Costcenter Costcenter;
    public readonly Product Product;
    public readonly Account Account;
    public readonly Subledger Subledger;
    public readonly BusinessType BusinessType;
    public readonly Money Money;
    public readonly Remark Remark;

    public PolicyBookingRow(
        int rowNumber,
        Policy policy,
        Costcenter costcenter,
        Product product,
        Account account,
        Subledger subledger,
        BusinessType businessType,
        Money amount,
        Remark remark)
    {
        RowNumber = rowNumber;
        Policy = policy;
        Costcenter = costcenter;
        Product = product;
        Account = account;
        Subledger = subledger;
        BusinessType = businessType;
        Money = amount;
        Remark = remark;
    }

    public PolicyBookingRow(int rowNumber)
    {
        RowNumber = rowNumber;
        Policy = new Policy();
        Costcenter = new Costcenter();
        Product = new Product();
        Account = new Account();
        Subledger = new Subledger();
        BusinessType = new BusinessType();
        Money = new Money();
        Remark = new Remark();
    }
}