using AutoMapper;
using Reservations.Domain.Shared;
using Reservations.Domain.StatusAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.Statuses
{
    public class StatusService : IStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StatusDto>> GetAsync()
        {
            var statuses = await _unitOfWork.StatusRepository.GetAsync();
            return _mapper.Map<List<Status>, List<StatusDto>>(statuses);
        }

        public async Task<StatusDto> GetAsync(int statusId)
        {
            var status = await _unitOfWork.StatusRepository.GetAsync(statusId);
            return _mapper.Map<Status, StatusDto>(status);
        }
    }
}
