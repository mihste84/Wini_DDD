namespace AppLogic.WiniLogic.Interfaces;

public interface IBankHolidayService
{
    Task<IEnumerable<DateTime>> GetBankHolidaysAsync();
}