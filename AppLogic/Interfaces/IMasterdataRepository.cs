namespace AppLogic.Interfaces;

public interface IMasterdataRepository
{
    Task<IEnumerable<Company>> SelectAllCompaniesAsync();
}