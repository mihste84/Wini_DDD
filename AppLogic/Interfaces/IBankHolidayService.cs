namespace AppLogic.Interfaces;

public interface IBankHolidayService
{
    Task<IEnumerable<DateTime>> GetBankHolidaysAsync();
}