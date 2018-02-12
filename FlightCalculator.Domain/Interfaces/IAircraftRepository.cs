using System.Collections.Generic;

namespace FlightCalculator.Core.Interfaces
{
    public interface IAircraftRepository
    {
        Aircraft GetAircraftById(int aircraftId);
        List<Aircraft> GetAllAircraft();
        void CreateAircraft(Aircraft aircraft);
        void DeleteAircraft(int aircraftId);
        void UpdateAircraft(Aircraft aircraft);
    }
}
