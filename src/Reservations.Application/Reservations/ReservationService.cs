using AutoMapper;
using Microsoft.Extensions.Logging;
using Reservations.Application.ChargingStationsMicroService.Chargers;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Reservations.Application.Reservations
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IChargerService _chargerService;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper, IChargerService chargerService, ILogger<ReservationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _chargerService = chargerService;
            _logger = logger;
        }

        public async Task<List<ReservationDto>> GetAsync(int? userId)
        {
            var endpoint = $"endpoint GET /Reservations?userId={userId}";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

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
                _logger.LogInformation($"Exited {endpoint} with: 200 OK");

                return reservationDtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }

        public async Task<ReservationDto> GetAsync(int reservationId)
        {
            var endpoint = $"endpoint GET /Reservations?{reservationId}";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

                var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);
                var reservationDto = _mapper.Map<Reservation, ReservationDto>(reservation);

                _logger.LogInformation($"Exited {endpoint} with: 200 OK");
                return reservationDto;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }

        public async Task<(ReservationDto, HttpStatusCode)> PostAsync(ReservationPostDto reservationPostDto)
        {
            var endpoint = $"endpoint POST /Reservations";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

                var reservations = await _unitOfWork.ReservationRepository.GetAsync();
                reservations = reservations.Where(x => x.ChargerId == reservationPostDto.ChargerId).ToList();

                var overlappingReservations =
                    reservations.Where(x => x.To > reservationPostDto.From && x.From < reservationPostDto.To);

                if (overlappingReservations.Any())
                {
                    _logger.LogInformation($"Exited {endpoint} with: 409 Reservation overlapping with an existing one");
                    return (null, HttpStatusCode.Conflict);
                }

                var reservation = _mapper.Map<ReservationPostDto, Reservation>(reservationPostDto);

                var (chargerDto, httpStatusCode) = await _chargerService.GetAsync(reservationPostDto.ChargerId);

                switch (httpStatusCode)
                {
                    case HttpStatusCode.NotFound:
                        _logger.LogInformation($"Exited {endpoint} with: 404 Charger with Id {reservationPostDto.ChargerId} not found");
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

                var reservationDto = _mapper.Map<Reservation, ReservationDto>(addedReservation);

                _logger.LogInformation($"Exited {endpoint} with: 201 Created");
                return (reservationDto, HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }

        public async Task<ReservationDto> PutAsync(int reservationId, ReservationPutDto reservationPutDto)
        {
            var endpoint = $"endpoint PUT /Reservations?{reservationId}";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

                var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);

                if (reservation == null)
                {
                    _logger.LogInformation($"Exited {endpoint} with: 404 Reservation with Id {reservationId} not found");
                    return null;
                }

                reservation.From = reservationPutDto.From;
                reservation.To = reservationPutDto.To;
                reservation.StatusId = reservationPutDto.StatusId;

                var updatedReservation = _unitOfWork.ReservationRepository.Update(reservation);
                await _unitOfWork.CommitAsync();
                var reservationDto = _mapper.Map<Reservation, ReservationDto>(updatedReservation);

                _logger.LogInformation($"Exited {endpoint} with: 200 OK");
                return reservationDto;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int reservationId)
        {
            var endpoint = $"endpoint DELETE /Reservations?{reservationId}";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

                var reservation = await _unitOfWork.ReservationRepository.GetAsync(reservationId);

                if (reservation == null)
                {
                    _logger.LogInformation($"Exited {endpoint} with: 404 Reservation with Id {reservationId} not found");
                    return false;
                }

                _unitOfWork.ReservationRepository.Remove(reservationId);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation($"Exited {endpoint} with: 200 OK");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }
    }
}
