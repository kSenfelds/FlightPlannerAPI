using System.Linq;
using Flight_plannerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flight_plannerAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerApiController : BaseApiController
    {
        public CustomerApiController(FlightPlannerDbContext context) : base(context)
        {
        }

        [HttpGet]
        [Route("airports")]
        public IActionResult GetAirports([FromQuery]string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return Ok(_context.Airports);
            }
            var result = _context.Airports.Where(a => a.AirportCode.ToLower().Contains(search.ToLower().Trim())
                                             || a.City.ToLower().Contains(search.ToLower().Trim())
                                             || a.Country.ToLower().Contains(search.ToLower().Trim())).ToArray();

            return Ok(result);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchRequest search)
        {
            if (IsValidSearch(search))
            {
                var result = Search(search);
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = _context.Flights.Include(f=> f.From).Include(f => f.To).SingleOrDefault(f=> f.Id == id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        private SearchModel Search(SearchRequest search)
        {
            SearchModel result = new SearchModel();

            result.Items = _context.Flights.Include(f => f.From).Include(f=> f.To).Where(f =>
                (f.From.AirportCode == search.From && f.To.AirportCode == search.To &&
                 f.DepartureTime == search.DepartureDate)
                || (f.From.City == search.From && f.To.City == search.To &&
                    f.DepartureTime == search.DepartureDate)
                || (f.From.Country == search.From && f.To.Country == search.To &&
                    f.DepartureTime == search.DepartureDate)).ToArray();
            result.TotalItems = result.Items.Length;
            result.Page = 0;

            return result;
        }

        public static bool IsValidSearch(SearchRequest search)
        {
            if (search == null || string.IsNullOrEmpty(search.From) || string.IsNullOrEmpty(search.To) || string.IsNullOrEmpty(search.DepartureDate))
            {
                return false;
            }

            if (search.To.ToLower() == search.From.ToLower())
            {
                return false;
            }

            return true;
        }
    }
}