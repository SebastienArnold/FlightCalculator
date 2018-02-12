using System.Collections.Generic;
using FlightCalculator.Core;

namespace FlightCalculator.Host.Models
{
    public class AircraftViewModel
    {
        public List<Aircraft> ExistingAircraft { get; set; }
        public Aircraft SelectedAircraft { get; set; }
    }
}
