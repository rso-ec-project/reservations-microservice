﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.Reservations
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetAsync();
        Task<ReservationDto> GetAsync(int statusId);
        Task<ReservationDto> PostAsync(ReservationPostDto reservationPostDto);
        Task<bool> DeleteAsync(int reservationId);
    }
}