using Reservations.Domain.StatusAggregate;

namespace Reservations.Infrastructure.Repositories
{
    public class StatusRepository : Repository<Status, int>, IStatusRepository
    {
        public StatusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
