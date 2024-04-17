namespace Domain.Factory;

using Domain.Model;

public class HolidayFactory: IHolidayFactory
{
    public Holiday NewHoliday(long id, long colaboratorId)
    {
        return new Holiday(id,colaboratorId);
    }
}