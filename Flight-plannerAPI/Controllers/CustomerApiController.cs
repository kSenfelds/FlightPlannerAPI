using Flight_plannerAPI.Models;
using Flight_plannerAPI.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Flight_plannerAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        [HttpGet]
        [Route("airports")]
        public IActionResult GetAirports([FromQuery]string search)
        {
            var airport = FlightStorage.GetAirport(search);

            return Ok(airport);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchRequest search)
        {
              bool isValid = FlightStorage.IsValidSearch(search);
              if (isValid)
              {
                  var result = FlightStorage.SearchFlights(search);
                  
                  return Ok(result);
              }

              return BadRequest();
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = FlightStorage.GetFlight(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }
    }
}