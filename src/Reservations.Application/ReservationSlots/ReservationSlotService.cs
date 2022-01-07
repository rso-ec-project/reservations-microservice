using Microsoft.Extensions.Logging;
using Reservations.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reservations.Application.ReservationSlots
{
    public class ReservationSlotService : IReservationSlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReservationSlotService> _logger;

        public ReservationSlotService(IUnitOfWork unitOfWork, ILogger<ReservationSlotService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ReservationSlotDto>> GetAsync(int chargerId, DateTime from, DateTime to)
        {
            var endpoint = $"endpoint GET /ReservationSlots?chargerId={chargerId}&from={from}&from={to}";

            try
            {
                _logger.LogInformation($"Entered {endpoint}");

                if (to < from)
                {
                    (to, @from) = (@from, to);
                }

                var reservations = await _unitOfWork.ReservationRepository.GetAsync();
                reservations = reservations.Where(x => x.To > from && x.From < to && x.ChargerId == chargerId).OrderBy(x => x.From).ToList();

                var reservationSlots = new List<ReservationSlotDto>();

                var nextSlotStartTime = from;

                foreach (var reservation in reservations)
                {
                    if (reservation.From < nextSlotStartTime)
                    {
                        nextSlotStartTime = reservation.To;
                    }
                    else
                    {
                        reservationSlots.Add(new ReservationSlotDto()
                        {
                            Duration = (int) reservation.From.Subtract(nextSlotStartTime).TotalMinutes,
                            From = nextSlotStartTime,
                            To = reservation.From,
                            ChargerId = chargerId
                        });

                        nextSlotStartTime = reservation.To;
                    }
                }

                if (nextSlotStartTime < to)
                {
                    reservationSlots.Add(new ReservationSlotDto()
                    {
                        Duration = (int) to.Subtract(nextSlotStartTime).TotalMinutes,
                        From = nextSlotStartTime,
                        To = to,
                        ChargerId = chargerId
                    });
                }

                reservationSlots = reservationSlots.Where(x => (x.To - x.From).TotalMinutes >= 15).ToList();
                
                _logger.LogInformation($"Exited {endpoint} with: 200 OK");
                return reservationSlots;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exited {endpoint} with: Exception {e.Message}");
                throw;
            }
        }
    }
}
