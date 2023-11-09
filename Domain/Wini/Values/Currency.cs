namespace Domain.Wini.Values;

public record Currency
{
    public CurrencyCode Code { get; }
    public decimal? CurrencyRate { get; }

    public Currency(CurrencyCode code, decimal? currencyRate = default)
    {
        Code = code;
        CurrencyRate = currencyRate;
    }
}