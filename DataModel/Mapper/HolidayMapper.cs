namespace DataModel.Mapper;

using DataModel.Model;
using Domain.Model;
using Domain.Factory;
using System.Linq;
using System;

public class HolidayMapper
{
    private IHolidayFactory _holidayFactory;
    private HolidayPeriodMapper _holidayPeriodMapper;

    private ColaboratorsIdMapper _colaboratorIdMapper;

    public HolidayMapper(
        IHolidayFactory holidayFactory,
        HolidayPeriodMapper holidayPeriodMapper,ColaboratorsIdMapper colaboratorIdMapper)
    {
        _holidayFactory = holidayFactory;
        _holidayPeriodMapper = holidayPeriodMapper;
        _colaboratorIdMapper = colaboratorIdMapper;
    }


    public Holiday ToDomain(HolidayDataModel holidayDM)
    {
        long id = holidayDM.Id;
        long colabId = _colaboratorIdMapper.ToDomain(holidayDM.colaboratorId) ;
        

        Holiday holidayDomain = _holidayFactory.NewHoliday(id,colabId);
        if(holidayDM.holidayPeriods!=null){
            foreach (var holidayPeriods in holidayDM.holidayPeriods)
            {
                IHolidayPeriodFactory _holidayPeriodFactory = new HolidayPeriodFactory();
                _holidayPeriodFactory.NewHolidayPeriod(holidayPeriods.StartDate, holidayPeriods.EndDate);
                holidayDomain.AddHolidayPeriod(_holidayPeriodFactory, holidayPeriods.StartDate, holidayPeriods.EndDate);
            }
        }
        return holidayDomain;
    }
 
    public IEnumerable<Holiday> ToDomain(IEnumerable<HolidayDataModel> holidaysDM)
    {
        List<Holiday> holidaysDomain = new List<Holiday>();

        foreach(HolidayDataModel holidayDataModel in holidaysDM)
        {
            Holiday holidayDomain = ToDomain(holidayDataModel);

            holidaysDomain.Add(holidayDomain);
        }

        return holidaysDomain.AsEnumerable();
    }

    

    public HolidayDataModel ToDataModel(Holiday holiday)
    {
        var holidayDataModel = new HolidayDataModel
        {
            Id = holiday.Id,
            colaboratorId = _colaboratorIdMapper.ToDataModel(holiday.GetColaborator()),
            holidayPeriods = holiday.GetHolidayPeriods().Select(hp => _holidayPeriodMapper.ToDataModel(hp)).ToList()
        };

        return holidayDataModel;
    }

    public bool AddHolidayPeriod(HolidayDataModel holidayDataModel, Holiday holidayDomain)
    {
        List<HolidayPeriodDataModel> lista = new List<HolidayPeriodDataModel>();

        var holidayPeriods = holidayDomain.GetHolidayPeriods();

        foreach(var holidayPeriod in holidayPeriods){
            var holidayPeriodDataModel = _holidayPeriodMapper.ToDataModel(holidayPeriod);
            lista.Add(holidayPeriodDataModel);
        }

        holidayDataModel.holidayPeriods = lista;

        return true;
    }

    public void UpdateHolidayPeriods(HolidayDataModel holidayDataModel, IEnumerable<HolidayPeriod> updatedPeriods)
    {
        // Converte os períodos existentes para um dicionário para acesso mais fácil
        var existingPeriodsDict = holidayDataModel.holidayPeriods
            .ToDictionary(hp => (hp.StartDate, hp.EndDate), hp => hp);

        foreach (var period in updatedPeriods)
        {
            // Cria uma chave para o período atual
            var periodKey = (period.StartDate, period.EndDate);

            // Se o período já existe, atualize-o; caso contrário, adicione como um novo
            if (existingPeriodsDict.ContainsKey(periodKey))
            {
                var existingPeriodDataModel = existingPeriodsDict[periodKey];
                // Atualize as propriedades de existingPeriodDataModel conforme necessário
                // Por exemplo: existingPeriodDataModel.SomeProperty = period.SomeProperty;
            }
            else
            {
                var periodDataModel = _holidayPeriodMapper.ToDataModel(period);
                holidayDataModel.holidayPeriods.Add(periodDataModel);
            }
        }
    }
}