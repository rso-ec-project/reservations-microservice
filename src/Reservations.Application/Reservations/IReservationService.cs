using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Reservations.Application.Reservations
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetAsync(int? userId);
        Task<ReservationDto> GetAsync(int reservationId);
        Task<(ReservationDto, HttpStatusCode)> PostAsync(ReservationPostDto reservationPostDto);
        Task<ReservationDto> PutAsync(int reservationId, ReservationPutDto reservationPutDto);
        Task<bool> DeleteAsync(int reservationId);
    }
}
