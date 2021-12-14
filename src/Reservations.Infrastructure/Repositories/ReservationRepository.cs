using Reservations.Domain.ReservationAggregate;

namespace Reservations.Infrastructure.Repositories
{
    public class ReservationRepository : Repository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
