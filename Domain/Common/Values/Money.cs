namespace Domain.Common.Values;

public readonly record struct Money
{
    public readonly decimal? Amount;
    public readonly Currency Currency;

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

    public bool IsDebitRow() => Amount > 0;
    public bool IsCreditRow() => Amount < 0;
    public bool IsCurrencyAndExchangeRateSet() => !string.IsNullOrWhiteSpace(Currency.CurrencyCode.Code) && Currency.ExchangeRate > 0;
}