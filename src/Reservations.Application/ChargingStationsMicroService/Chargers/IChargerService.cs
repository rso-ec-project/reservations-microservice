using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Reservations.Application.ChargingStationsMicroService.Chargers
{
    public interface IChargerService
    {
        Task<List<ChargerDto>> GetAsync(List<int> chargerIds);
        Task<(ChargerDto, HttpStatusCode?)> GetAsync(int chargerId);
    }
}
