using System.Collections.Generic;
using FlightCalculator.Core;

namespace FlightCalculator.Host.Models
{
    public class ReportViewModel
    {
        public List<Aircraft> Aircraft { get; set; }

        public List<Flight> Flights { get; set; }
    }
}
