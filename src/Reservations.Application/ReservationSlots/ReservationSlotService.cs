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

        public ReservationSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ReservationSlotDto>> GetAsync(int chargerId, DateTime from, DateTime to)
        {
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

            return reservationSlots;
        }
    }
}
