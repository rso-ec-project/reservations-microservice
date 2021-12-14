using Reservations.Domain.Shared;

namespace Reservations.Domain.StatusAggregate
{
    public interface IStatusRepository : IRepository<Status, int>
    {
    }
}
