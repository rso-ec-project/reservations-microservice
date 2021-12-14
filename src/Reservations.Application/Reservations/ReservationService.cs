using AutoMapper;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Application.Reservations
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ReservationDto>> GetAsync()
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();
            return _mapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
        }

        public async Task<ReservationDto> GetAsync(int reservationId)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);
            return _mapper.Map<Reservation, ReservationDto>(reservation);
        }

        public async Task<ReservationDto> PostAsync(ReservationPostDto reservationPostDto)
        {
            var reservation = _mapper.Map<ReservationPostDto, Reservation>(reservationPostDto);
            var addedReservation = await _unitOfWork.ReservationRepository.AddAsync(reservation);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<Reservation, ReservationDto>(addedReservation);
        }

        public async Task<bool> DeleteAsync(int reservationId)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);

            if (reservation == null)
                return false;

            _unitOfWork.ReservationRepository.Remove(reservationId);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
