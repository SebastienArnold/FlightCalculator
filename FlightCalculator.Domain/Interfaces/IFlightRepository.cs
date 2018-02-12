using System.Collections.Generic;

namespace FlightCalculator.Core.Interfaces
{
    public interface IFlightRepository
    {
        Flight GetFlightById(int flightId);
        List<Flight> GetAllFlights();
        void CreateFlight(Flight flight);
        void DeleteFlight(int aircraftId);
        void UpdateFlight(Flight flight);
    }
}
