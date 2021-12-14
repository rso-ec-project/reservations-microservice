using Reservations.Domain.Shared.Entities;
using Reservations.Domain.StatusAggregate;
using System;

namespace Reservations.Domain.ReservationAggregate
{
    public class Reservation : Entity<int>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Code { get; set; }
        public int UserId { get; set; }
        public int ChargerId { get; set; }
        public int StatusId { get; set; }

        public virtual Status Status { get; set; }
    }
}
