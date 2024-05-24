namespace Domain.Wini.Values;

public record BookingRow
{
    public readonly int RowNumber;
    public readonly BusinessUnit BusinessUnit;
    public readonly Account Account;
    public readonly Subledger Subledger;
    public readonly CostObject CostObject1;
    public readonly CostObject CostObject2;
    public readonly CostObject CostObject3;
    public readonly CostObject CostObject4;
    public readonly Remark Remark;
    public readonly Authorizer Authorizer;
    public readonly Money Money;

    public BookingRow(
        int rowNumber,
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
        Money = amount;
    }

    public BookingRow(int rowNumber)
    {
        RowNumber = rowNumber;
        BusinessUnit = new BusinessUnit();
        Account = new Account();
        Subledger = new Subledger();
        CostObject1 = new CostObject(1);
        CostObject2 = new CostObject(2);
        CostObject3 = new CostObject(3);
        CostObject4 = new CostObject(4);
        Remark = new Remark();
        Authorizer = new Authorizer();
        Money = new Money();
    }

    public BookingRow ChangeRowAuthorization(bool hasBeenAuthorized)
    => new(
        RowNumber,
        BusinessUnit,
        Account,
        Subledger,
        CostObject1,
        CostObject2,
        CostObject3,
        CostObject4,
        Remark,
        new Authorizer(Authorizer.UserId, hasBeenAuthorized),
        Money
    );

    public bool IsCompanyCodeAllowed(IEnumerable<Company> companies)
    => companies.Select(_ => _.CompanyCode).Contains(BusinessUnit.CompanyCode);

    public bool IsBaseCurrencyUsed(IEnumerable<Company> companies)
    => companies.Any(_ => _.CompanyCode == BusinessUnit.CompanyCode && _.Currency.Code == Money.Currency.CurrencyCode.Code);

    public bool CanRowBeAuthorized()
    => Money.IsDebitRow() && !Authorizer.HasAuthorized && !string.IsNullOrWhiteSpace(Authorizer.UserId);

    public bool CanRowBeAuthorizedByUser(string userId)
    => CanRowBeAuthorized() && Authorizer.UserId == userId;
}