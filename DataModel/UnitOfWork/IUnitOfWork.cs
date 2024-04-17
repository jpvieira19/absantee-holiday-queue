using Domain.IRepository;

namespace UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository Generic { get; }

    IHolidayRepository HolidayRepository { get; }

    IHolidayPeriodRepository HolidayPeriodRepository { get; }

    Task<int> CompleteAsync();
}
