namespace FlightCalculator.Core
{
    public class Flight
    {
        public int Id { get; set; }
        public Airport FromAirport { get; set; }
        public Airport ToAirport { get; set; }
        public Aircraft Aircraft { get; set; }
        public double FuelConsumption { get; set; }
        public double Distance { get; set; }
    }
}
