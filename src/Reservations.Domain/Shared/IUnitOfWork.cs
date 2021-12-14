using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.StatusAggregate;
using System.Threading.Tasks;

namespace Reservations.Domain.Shared
{
    public interface IUnitOfWork
    {
        IUnitOfWork CreateContext();

        IReservationRepository ReservationRepository { get; }

        IStatusRepository StatusRepository { get; }

        Task<int> CommitAsync();
    }
}
