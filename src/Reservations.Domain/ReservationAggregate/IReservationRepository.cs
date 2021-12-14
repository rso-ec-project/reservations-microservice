using Reservations.Domain.Shared;

namespace Reservations.Domain.ReservationAggregate
{
    public interface IReservationRepository : IRepository<Reservation, int>
    {
    }
}
