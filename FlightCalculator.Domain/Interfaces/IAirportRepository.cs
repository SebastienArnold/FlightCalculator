using System.Collections.Generic;

namespace FlightCalculator.Core.Interfaces
{
    public interface IAirportRepository
    {
        List<string> GetAllCountries();
        List<Airport> GetAllAirport(string country);
        Airport GetAirportByCode(string airportCode);
    }
}
