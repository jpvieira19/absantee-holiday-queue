namespace Domain.IRepository;

using Domain.Model;

public interface IHolidayPeriodRepository : IGenericRepository<HolidayPeriod>
{
    Task<IEnumerable<HolidayPeriod>> GetHolidayPeriodsAsync();

    Task<HolidayPeriod> GetHolidayPeriodByIdAsync(long id);

    Task<HolidayPeriod> Add(HolidayPeriod holidayPeriod);

    Task<bool> HolidayPeriodExists(long id);
    
}
