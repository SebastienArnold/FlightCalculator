using System.Collections.Generic;
using FlightCalculator.Core;

namespace FlightCalculator.Host.Models
{
    public class FlightViewModel
    {
        public List<string> Countries { get; set; }
        public List<Aircraft> ExistingAircraft { get; set; }
        public List<Flight> ExistingFlights { get; set; }
        public Flight SelectedFlight { get; set; }

        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
    }
}
