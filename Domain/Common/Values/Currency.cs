namespace Domain.Common.Values;

public readonly record struct Currency
{
    public readonly CurrencyCode CurrencyCode;
    public readonly decimal? ExchangeRate;
    public Currency()
    {
        CurrencyCode = new CurrencyCode();
    }
    public Currency(CurrencyCode code, decimal? exchangeRate = 0)
    {
        CurrencyCode = code;
        ExchangeRate = exchangeRate;
    }
}