namespace FlightCalculator.Core.Interfaces
{
    public interface IFlightCalculator
    {
        double GetDistance(GeoDMS sourceLatitude, GeoDMS sourceLongitude,
            GeoDMS destinationLatitude, GeoDMS destinationLongitude);

        double GetConsumption(double distance, double consumptionPerKilometer, double takeoffEffort);
    }
}
