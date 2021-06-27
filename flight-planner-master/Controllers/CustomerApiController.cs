using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    public class CustomerApiController : ApiController
    {
        private static readonly object Locker = new object();

        [Route("api/airports")]
        [HttpGet]
        public IHttpActionResult SearchAirports(string search)
        {
            lock (Locker)
            {
                search = search.ToLower().Trim();
                var airportList = new List<Airport>();

                foreach (var f in FlightStorage.AllFlights)
                {
                    if (f.To.AirportName.ToLower().Contains(search) ||
                        f.To.City.ToLower().Contains(search) ||
                        f.To.Country.ToLower().Contains(search))
                    {
                        AirportStorage.AddAirport(f.To);
                        var toAirport = f.To;
                        airportList.Add(toAirport);
                    }

                    if (f.From.AirportName.ToLower().Contains(search) ||
                        f.From.City.ToLower().Contains(search) ||
                        f.From.Country.ToLower().Contains(search))
                    {
                        AirportStorage.AddAirport(f.From);
                        var fromAirport = f.From;
                        airportList.Add(fromAirport);
                    }
                }

                return AirportStorage.AllAirports.Count == 0 ? (IHttpActionResult) NotFound() : Ok(airportList);
            }
        }

        [Route("api/flights/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetFlightById(int id)
        {
            var flight = FlightStorage.FindFlight(id);
            return flight == null ? (IHttpActionResult) NotFound() : Ok(flight);
        }

        [Route("api/flights/search")]
        [HttpPost]
        public IHttpActionResult FindFlight([FromBody] SearchFlightsRequest request)
        {
            lock (Locker)
            {
                if (IsSomeOfFlightsParametersNull(request) || IsToAndFromAirportsTheSame(request))
                {
                    return BadRequest();
                }

                var flights = FlightStorage.AllFlights.Where(f =>
                    f.From.AirportName == request.From &&
                    f.To.AirportName == request.To &&
                    f.DepartureTime.Substring(0, 10) == request.DepartureDate).ToList();

                var page = new PageResult {Page = 0, TotalItems = flights.Count(), Items = flights};

                return Ok(page);
            }
        }

        private static bool IsSomeOfFlightsParametersNull(SearchFlightsRequest flight)
        {
            if (flight == null)
            {
                return true;
            }

            return (
                string.IsNullOrEmpty(flight.To) || string.IsNullOrEmpty(flight.DepartureDate) ||
                string.IsNullOrEmpty(flight.From));
        }

        private static bool IsToAndFromAirportsTheSame(SearchFlightsRequest flight)
        {
            return flight.From == flight.To;
        }
    }
}