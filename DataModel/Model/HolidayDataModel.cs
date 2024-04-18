using DataModel.Mapper;
using DataModel.Model;
using Domain.Model;

public class HolidayDataModel
{
    public long Id { get; set; }
    public ColaboratorsIdDataModel colaboratorId { get; set; }
    public List<HolidayPeriodDataModel> holidayPeriods { get; set; }

    public HolidayDataModel() {}

    public HolidayDataModel(Holiday holiday,HolidayPeriodMapper holidayPeriodMapper,ColaboratorsIdMapper colaboratorsIdMapper)
    {
        Id = holiday.Id;

        // Assuming you have a mapper that converts from the domain Colaborator model to the data model
        colaboratorId = colaboratorsIdMapper.ToDataModel(holiday.GetColaborator());
        
        // And assuming your holiday periods need to be converted as well
        holidayPeriods = holiday.GetHolidayPeriods().Select(hp => holidayPeriodMapper.ToDataModel(hp)).ToList();
    }
}
