namespace DataModel.Mapper;

using DataModel.Model;

using Domain.Model;
using Domain.Factory;

public class HolidayPeriodMapper
{
    private IHolidayPeriodFactory _holidayPeriodFactory;

    public HolidayPeriodMapper(IHolidayPeriodFactory holidayPeriodFactory)
    {
        _holidayPeriodFactory = holidayPeriodFactory;
    }

    public HolidayPeriod ToDomain(HolidayPeriodDataModel holidayPeriodDM)
    {
        HolidayPeriod holidayPeriodDomain = _holidayPeriodFactory.NewHolidayPeriod(holidayPeriodDM.StartDate,holidayPeriodDM.EndDate);

        holidayPeriodDomain.Id = holidayPeriodDM.Id;

        return holidayPeriodDomain;
    }

    public IEnumerable<HolidayPeriod> ToDomain(IEnumerable<HolidayPeriodDataModel> holidayPeriodsDataModel)
    {

        List<HolidayPeriod> holidayPeriodsDomain = new List<HolidayPeriod>();

        foreach(HolidayPeriodDataModel holidayPeriodDataModel in holidayPeriodsDataModel)
        {
            HolidayPeriod holidayPeriodDomain = ToDomain(holidayPeriodDataModel);

            holidayPeriodsDomain.Add(holidayPeriodDomain);
        }

        return holidayPeriodsDomain.AsEnumerable();
    }

    public HolidayPeriodDataModel ToDataModel(HolidayPeriod holidayPeriod)
    {
        HolidayPeriodDataModel holidayPeriodDataModel = new HolidayPeriodDataModel(holidayPeriod);

        return holidayPeriodDataModel;
    }

}