using System;
using System.Linq;
using System.Web.Http;
using FlightPlanner.Attributes;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    [BasicAuthentication]
    public class AdminApiController : ApiController
    {
        private static readonly object Locker = new object();

        [Route("admin-api/flights/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetFlights(int id)
        {
            lock (Locker)
            {
                var flight = FlightStorage.FindFlight(id);
                return flight == null ? (IHttpActionResult) NotFound() : Ok();
            }
        }

        [Route("admin-api/flights")]
        [HttpPut]
        public IHttpActionResult PutFlight(AddFlightRequest newFlight)
        {
            lock (Locker)
            {
                 if (IsFlightsParametersNull(newFlight) || IsToAndFromAirportsTheSame(newFlight) || IsArrivalTimeBeforeDepartureTime(newFlight))
                 {
                     return BadRequest();
                 }
                 
                 var flightToBeAdded = new Flight
                 {
                     ArrivalTime = newFlight.ArrivalTime,
                     DepartureTime = newFlight.DepartureTime,
                     Carrier = newFlight.Carrier,
                     From = new Airport()
                     {
                         City = newFlight.From.City,
                         Country = newFlight.From.Country,
                         AirportName = newFlight.From.AirportName
                     },
                     To = new Airport()
                     {
                         City = newFlight.To.City,
                         Country = newFlight.To.Country,
                         AirportName = newFlight.To.AirportName
                     }
                 };

                 if (!IsNotInStorage(newFlight))
                 {
                     FlightStorage.AddFlight(flightToBeAdded);
                 }

                 else
                 {
                     return Conflict();
                 }
                 
                 return Created(string.Empty, flightToBeAdded);
            }
        }

        private static bool IsFlightsParametersNull(AddFlightRequest flightToBeAdded)
        {
            return (string.IsNullOrEmpty(flightToBeAdded.Carrier) ||
                    string.IsNullOrEmpty(flightToBeAdded.DepartureTime) ||
                    string.IsNullOrEmpty(flightToBeAdded.ArrivalTime) ||
                    flightToBeAdded.To == null ||
                    flightToBeAdded.From == null ||
                    string.IsNullOrEmpty(flightToBeAdded.To?.AirportName) ||
                    string.IsNullOrEmpty(flightToBeAdded.To?.City) ||
                    string.IsNullOrEmpty(flightToBeAdded.To?.Country) ||
                    string.IsNullOrEmpty(flightToBeAdded.From?.AirportName) ||
                    string.IsNullOrEmpty(flightToBeAdded.From?.City) ||
                    string.IsNullOrEmpty(flightToBeAdded.From?.Country));
        }

        private static bool IsToAndFromAirportsTheSame(AddFlightRequest newFlight)
        {
            return newFlight.To.AirportName.ToLower().Trim() == newFlight.From.AirportName.ToLower().Trim();
        }

        private static bool IsArrivalTimeBeforeDepartureTime(AddFlightRequest newFlight)
        {
            return DateTime.Parse(newFlight.ArrivalTime) <= DateTime.Parse(newFlight.DepartureTime);
        }

        private static bool IsNotInStorage(AddFlightRequest newFlight)
        {
            return FlightStorage.AllFlights.Any(f =>
            {
                if (f.ArrivalTime != newFlight.ArrivalTime || f.DepartureTime != newFlight.DepartureTime)
                {
                    return false;
                }

                if (f.Carrier.ToLower().Trim() != newFlight.Carrier.ToLower().Trim())
                {
                    return false;
                }

                if (f.From.AirportName.ToLower().Trim() != newFlight.From.AirportName.ToLower().Trim())
                {
                    return false;
                }

                return true;

            });
        }
        
        [Route("admin-api/flights/{id:int}")]
        [HttpDelete]
        public IHttpActionResult DeleteFlight(int id)
        {
            lock (Locker)
            {
                FlightStorage.DeleteFlight(id);
                return Ok();
            }
        }
    }
}