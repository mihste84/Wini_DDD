namespace AppLogic.Interfaces;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> SelectAllCompaniesAsync();
}