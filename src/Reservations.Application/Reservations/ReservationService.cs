using AutoMapper;
using Reservations.Application.ChargingStationsMicroService.Chargers;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Reservations.Application.Reservations
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IChargerService _chargerService;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper, IChargerService chargerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _chargerService = chargerService;
        }

        public async Task<List<ReservationDto>> GetAsync(int? userId)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();

            if (userId != null)
                reservations = reservations.Where(x => x.UserId == userId).ToList();

            var reservationDtos = _mapper.Map<List<Reservation>, List<ReservationDto>>(reservations);
            foreach (var reservation in reservationDtos)
            {
                var (charger, httpStatusCode) = await _chargerService.GetAsync(reservation.ChargerId);

                reservation.Charger = httpStatusCode switch
                {
                    HttpStatusCode.NotFound => new ChargerDto(),
                    HttpStatusCode.OK => charger,
                    _ => null
                };
            }

            return reservationDtos;
        }

        public async Task<ReservationDto> GetAsync(int reservationId)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);
            return _mapper.Map<Reservation, ReservationDto>(reservation);
        }

        public async Task<(ReservationDto, HttpStatusCode)> PostAsync(ReservationPostDto reservationPostDto)
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();
            reservations = reservations.Where(x => x.ChargerId == reservationPostDto.ChargerId).ToList();

            var overlappingReservations =
                reservations.Where(x => x.To > reservationPostDto.From && x.From < reservationPostDto.To);

            if (overlappingReservations.Any())
                return (null, HttpStatusCode.Conflict);

            var reservation = _mapper.Map<ReservationPostDto, Reservation>(reservationPostDto);

            var (chargerDto, httpStatusCode) = await _chargerService.GetAsync(reservationPostDto.ChargerId);

            switch (httpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    return (null, HttpStatusCode.NotFound);
                case HttpStatusCode.OK when chargerDto.Id == reservationPostDto.ChargerId:
                    reservation.StatusId = 1;
                    break;
                default:
                    reservation.StatusId = 2;
                    break;
            }

            var addedReservation = await _unitOfWork.ReservationRepository.AddAsync(reservation);
            await _unitOfWork.CommitAsync();
            return (_mapper.Map<Reservation, ReservationDto>(addedReservation), HttpStatusCode.Created);
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
