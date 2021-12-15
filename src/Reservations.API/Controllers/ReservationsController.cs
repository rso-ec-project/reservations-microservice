using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Reservations;
using System.Collections.Generic;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ReservationDto>>> Get()
        {
            return await _reservationService.GetAsync();
        }

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReservationDto>> Post([FromBody] ReservationPostDto reservationPostDto)
        {
            return await _reservationService.PostAsync(reservationPostDto);
        }

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
