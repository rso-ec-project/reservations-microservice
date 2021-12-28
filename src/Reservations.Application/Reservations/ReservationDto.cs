using Reservations.Application.ChargingStationsMicroService.Chargers;
using System;

namespace Reservations.Application.Reservations
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Code { get; set; }
        public int UserId { get; set; }
        public int ChargerId { get; set; }
        public int StatusId { get; set; }
        public ChargerDto Charger { get; set; }
    }
}
