using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Models
{
    public static class FlightStorage
    {
        public static List<Flight> AllFlights = new List<Flight>();

        private static int _id;

        public static Flight AddFlight(Flight newFlight)
        {
            newFlight.Id = _id;
            _id++;
            AllFlights.Add(newFlight);
            return newFlight;
        }

        public static Flight FindFlight(int id)
        {
            return AllFlights.FirstOrDefault(f => f.Id == id);
        }

        public static void DeleteFlight(int id)
        {
            AllFlights.Remove(FindFlight(id));
        }
    }
}