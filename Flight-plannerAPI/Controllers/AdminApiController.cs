using System;
using System.Linq;
using Flight_plannerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flight_plannerAPI.Controllers
{
    [Route("admin-api")]
    [ApiController]
    [Authorize]
    
    public class AdminApiController : BaseApiController
    {
        private readonly object _lock = new object();
        public AdminApiController(FlightPlannerDbContext context) : base(context)
        {
        }
        
        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlights(int id)
        {
            var flight = _context.Flights.Include(f => f.From)
                .Include(f => f.To)
                .SingleOrDefault(f => f.Id == id);
            if (flight == null)
                return NotFound();
            return Ok(flight);
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult AddFlight(Flight flight)
        {
            if (!IsValidFlight(flight))
                return BadRequest();

            lock (_lock)
            {
                if (FlightExists(flight))
                    return Conflict();

            
                _context.Flights.Add(flight);
                _context.SaveChanges();
                    return Created("", flight);
            }
        }

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            lock (_lock)
            {
                var flight = _context.Flights.SingleOrDefault(f => f.Id == id);

                if (flight == null)
                {
                    return Ok();
                }

                _context.Flights.Remove(flight);
                _context.SaveChanges();
                return Ok();
            }
        }

        private bool IsValidFlight(Flight flight)
        {
            if (flight == null || flight.From == null || flight.To == null)
            {
                return false;
            }

            if (
                string.IsNullOrEmpty(flight.From.AirportCode)
                || string.IsNullOrEmpty(flight.From.City)
                || string.IsNullOrEmpty(flight.From.Country)
                || string.IsNullOrEmpty(flight.DepartureTime)
                || string.IsNullOrEmpty(flight.ArrivalTime)
                || string.IsNullOrEmpty(flight.Carrier)
                || string.IsNullOrEmpty(flight.To.AirportCode)
                || string.IsNullOrEmpty(flight.To.City)
                || string.IsNullOrEmpty(flight.To.Country))
            {
                return false;
            }

            if (
                flight.To.AirportCode.ToLower() == flight.From.AirportCode.ToLower()
                || flight.To.City.ToLower() == flight.From.City.ToLower())
            {
                return false;
            }

            DateTime departure = Convert.ToDateTime(flight.DepartureTime);
            DateTime arrival = Convert.ToDateTime(flight.ArrivalTime);

            if (departure >= arrival)
            {
                return false;
            }

            return true;
        }

        private bool FlightExists(Flight flight)
        {
            return _context.Flights.Any(f => f.From.City.ToLower() == flight.From.City.ToLower()
                                             && f.To.City.ToLower() == flight.To.City.ToLower()
                                             && f.Carrier.ToLower() == flight.Carrier.ToLower()
                                             && f.DepartureTime == flight.DepartureTime &&
                                             f.ArrivalTime == flight.ArrivalTime);
        }
    }
}