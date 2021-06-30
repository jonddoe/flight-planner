using System.Collections.Generic;

namespace FlightPlanner.Models
{
    public static class AirportStorage
    {
        public static List<Airport> AllAirports = new List<Airport>();

        public static Airport AddAirport(Airport newAirport)
        {
            AllAirports.Add(newAirport);
            return newAirport;
        }
    }
}