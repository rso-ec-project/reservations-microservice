using AutoMapper;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<ReservationDto>> GetAsync(int? userId)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();

            if (userId != null)
                reservations = reservations.Where(x => x.UserId == userId).ToList();

            return _mapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
        }

        public async Task<ReservationDto> GetAsync(int reservationId)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);
            return _mapper.Map<Reservation, ReservationDto>(reservation);
        }

        public async Task<ReservationDto> PostAsync(ReservationPostDto reservationPostDto)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();
            reservations = reservations.Where(x => x.ChargerId == reservationPostDto.ChargerId).ToList();

            var overlappingReservations =
                reservations.Where(x => x.To > reservationPostDto.From && x.From < reservationPostDto.To);

            if (overlappingReservations.Any())
                return null;

            var reservation = _mapper.Map<ReservationPostDto, Reservation>(reservationPostDto);
            var addedReservation = await _unitOfWork.ReservationRepository.AddAsync(reservation);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<Reservation, ReservationDto>(addedReservation);
        }

        public async Task<ReservationDto> PutAsync(int reservationId, ReservationPutDto reservationPutDto)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);

            if (reservation == null)
                return null;

            reservation.From = reservationPutDto.From;
            reservation.To = reservationPutDto.To;
            reservation.StatusId = reservationPutDto.StatusId;

            var updatedReservation = _unitOfWork.ReservationRepository.Update(reservation);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<Reservation, ReservationDto>(updatedReservation);
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
