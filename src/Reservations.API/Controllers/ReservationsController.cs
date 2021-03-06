using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Reservations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Reservations.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Get a list of user reservations.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ReservationDto>>> Get([FromQuery] int? userId = null)
        {
            return await _reservationService.GetAsync(userId);
        }

        /// <summary>
        /// Get a single reservation by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservationDto>> Get(int id)
        {
            var reservation = await _reservationService.GetAsync(id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        /// <summary>
        /// Create a reservation.
        /// </summary>
        /// <param name="reservationPostDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReservationDto>> Post([FromBody] ReservationPostDto reservationPostDto)
        {
            var (reservation, httpStatusCode) = await _reservationService.PostAsync(reservationPostDto);

            return httpStatusCode switch
            {
                HttpStatusCode.Conflict => Conflict("Posted reservation is overlapping with an existing reservation."),
                HttpStatusCode.NotFound => NotFound($"Charger with Id {reservationPostDto.ChargerId} not found."),
                _ => reservation
            };
        }

        /// <summary>
        /// Update a reservation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reservationPutDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReservationDto>> Put(int id, [FromBody] ReservationPutDto reservationPutDto)
        {
            var reservation = await _reservationService.PutAsync(id, reservationPutDto);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        /// <summary>
        /// Delete a reservation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int id)
        {
             var isDeleted = await _reservationService.DeleteAsync(id);

             if (!isDeleted)
                 return NotFound();
             return Ok();
        }
    }
}
