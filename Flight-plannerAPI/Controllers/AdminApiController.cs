using Flight_plannerAPI.Models;
using Flight_plannerAPI.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flight_plannerAPI.Controllers
{
    [Route("admin-api")]
    [ApiController]
    [Authorize]
    
    public class AdminApiController : ControllerBase
    {

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);
            if (flight == null)
                return NotFound();
            return Ok(flight);
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight(Flight flight)
        {
            var currentFlight = FlightStorage.AddFlight(flight);
            if (currentFlight.Id == 0)
                return BadRequest("Wrong values entered");
            if (currentFlight.Id == -1)
            {
                return Conflict();
            }

            return Created("", flight);  
        }

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
           var flight = FlightStorage.GetFlight(id);
           
            if (flight != null)
            {
                FlightStorage.DeleteFlight(flight.Id);
                return Ok();
            }
           
           return Ok();
        }
    }
}