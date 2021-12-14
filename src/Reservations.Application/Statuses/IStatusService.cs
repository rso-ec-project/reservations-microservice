using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.Statuses
{
    public interface IStatusService
    {
        Task<List<StatusDto>> GetAsync();
        Task<StatusDto> GetAsync(int statusId);
    }
}
