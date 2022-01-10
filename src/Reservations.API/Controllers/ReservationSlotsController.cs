using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.ReservationSlots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Reservations.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ReservationSlotsController : ControllerBase
    {
        private readonly IReservationSlotService _reservationSlotService;

        public ReservationSlotsController(IReservationSlotService reservationSlotService)
        {
            _reservationSlotService = reservationSlotService;
        }

        /// <summary>
        /// Get available reservation slots of a charger between two dates.
        /// </summary>
        /// <param name="chargerId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ReservationSlotDto>>> Get([FromQuery, Required] int chargerId, [FromQuery, Required] DateTime from, [FromQuery, Required] DateTime to)
        {
            return await _reservationSlotService.GetAsync(chargerId, from, to);
        }
    }
}
