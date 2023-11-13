namespace Domain.Wini.Entities;

public class Company
{
    public readonly IdValue<int> Id;
    public CompanyCode CompanyCode { get; set; }
    public CompanyName Name { get; set; }
    public Country Country { get; set; }
    public readonly CurrencyCode Currency;
    public Company(IdValue<int> id, CompanyCode companyCode, CompanyName name, Country country)
    {
        Id = id;
        CompanyCode = companyCode;
        Name = name;
        Country = country;
        Currency = new CurrencyCode(country);
    }
}