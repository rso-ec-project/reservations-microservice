using System;

namespace Reservations.Application.Reservations
{
    public class ReservationPutDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int StatusId { get; set; }
    }
}
