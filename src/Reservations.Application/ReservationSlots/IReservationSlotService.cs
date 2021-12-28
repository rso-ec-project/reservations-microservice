using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.ReservationSlots
{
    public interface IReservationSlotService
    {
        Task<List<ReservationSlotDto>> GetAsync(int chargerId, DateTime from, DateTime to);
    }
}
