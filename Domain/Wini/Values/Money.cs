namespace Domain.Wini.Values;

public record Money
{
    public decimal? Amount { get; }
    public Currency Currency { get; }

    public Money()
    {
        Amount = 0;
        Currency = new Currency();
    }

    public Money(decimal? amount, string? currencyCodeString, decimal? exchangeRate)
    {
        var currencyCode = new CurrencyCode(currencyCodeString);
        Amount = amount;
        Currency = new Currency(currencyCode, exchangeRate);
    }

    public Money(decimal? amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public bool IsDebitRow() => Amount >= 0;
    public bool IsCreditRow() => Amount < 0;
}