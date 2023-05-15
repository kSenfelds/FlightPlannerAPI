using System;
using System.Collections.Generic;
using System.Linq;
using Flight_plannerAPI.Models;

namespace Flight_plannerAPI.Storage
{
    public static class  FlightStorage
    {
        private static List<Flight> _flights  = new List<Flight>();
        private static readonly object _lock = new object();
        private static int _id = 1;
        public static Flight  GetFlight(int id)
        {
            return _flights.SingleOrDefault(f => f.Id == id);
        }

        public static Airport [] GetAirport(string phrase)
        {
            var airports = _flights.SelectMany(f => new[] { f.From, f.To }).ToList();
            if (string.IsNullOrEmpty(phrase))
            {
                return airports.ToArray();
            }
            var result = airports.Where(a => a.AirportCode.ToLower().Contains(phrase.ToLower().Trim())
                                             || a.City.ToLower().Contains(phrase.ToLower().Trim())
                                             || a.Country.ToLower().Contains(phrase.ToLower().Trim())).ToArray();

            return result;
        }

        public static bool FlightExists(Flight flight)
        {
            lock (_lock)
            {
                return _flights.Exists(f => f.From.AirportCode == flight.From.AirportCode && f.To.AirportCode == flight.To.AirportCode && f.DepartureTime == flight.DepartureTime);
            }
        }

        public static Flight AddFlight(Flight flight)
        {
            lock (_lock)
            {
                if (IsValidFlight(flight) && !FlightExists(flight))
                {
                    flight.Id = _id++;
                    _flights.Add(flight);
                    return flight;
                }

                if (FlightExists(flight))
                {
                    flight.Id = -1;
                    return flight;
                }

                flight.Id = 0;
                return flight;
            }
        }

        public static void Clear()
        {
            _flights.Clear();
        }

        public static void DeleteFlight(int id)
        {
            lock (_lock)
            {
                var flight = _flights.SingleOrDefault(f => f.Id == id);
                _flights.Remove(flight);
            }
        }

        public static bool IsValidFlight(Flight flight)
        {
            lock (_lock)
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
        }

        public static SearchModel SearchFlights(SearchRequest search)
        {
            SearchModel result = new SearchModel();

            result.Items = _flights.Where(f =>
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

            if (
                search.To.ToLower() == search.From.ToLower())
                
            {
                return false;
            }

            return true;
        }
    }
}