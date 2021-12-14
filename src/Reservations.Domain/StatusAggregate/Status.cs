using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared.Entities;
using System.Collections.Generic;

namespace Reservations.Domain.StatusAggregate
{
    public class Status : Entity<int>
    {
        public string Name { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
