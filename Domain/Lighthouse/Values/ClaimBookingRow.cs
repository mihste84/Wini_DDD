namespace Domain.Lighthouse.Values;

public record ClaimBookingRow : PolicyBookingRow
{
    public readonly Claim Claim;
    public ClaimBookingRow(
        int rowNumber,
        Claim claim,
        Policy policy,
        Costcenter costcenter,
        Product product,
        Account account,
        Subledger subledger,
        BusinessType businessType,
        Money amount,
        Remark remark) : base(rowNumber, policy, costcenter, product, account, subledger, businessType, amount, remark)
    {
        Claim = claim;
    }

    public ClaimBookingRow(int rowNumber) : base(rowNumber)
    {
        Claim = new Claim();
    }
}