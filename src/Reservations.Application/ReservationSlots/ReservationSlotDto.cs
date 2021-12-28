using System;

namespace Reservations.Application.ReservationSlots
{
    public class ReservationSlotDto
    {
        public int ChargerId { get; set; }
        public int Duration { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
