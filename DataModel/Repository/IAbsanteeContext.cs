namespace DataModel.Repository;

using Microsoft.EntityFrameworkCore;

using DataModel.Model;

public interface IAbsanteeContext
{
	DbSet<HolidayDataModel> Holidays { get; set; }
	DbSet<HolidayPeriodDataModel> HolidayPeriods { get; set; }

}