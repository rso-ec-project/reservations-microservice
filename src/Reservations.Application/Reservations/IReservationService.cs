using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.Reservations
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetAsync();
        Task<ReservationDto> GetAsync(int reservationId);
        Task<ReservationDto> PostAsync(ReservationPostDto reservationPostDto);
        Task<ReservationDto> PutAsync(int reservationId, ReservationPutDto reservationPutDto);
        Task<bool> DeleteAsync(int reservationId);
    }
}
