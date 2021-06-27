using System.Web.Http;
using FlightPlanner.Models;

namespace FlightPlanner.Controllers
{
    public class TestingApiController : ApiController
    {
        [Route("testing-api/clear"), HttpPost]
        public IHttpActionResult Clear()
        {
            FlightStorage.AllFlights.Clear();
            AirportStorage.AllAirports.Clear();
            return Ok();
        }
    }
}
