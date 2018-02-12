namespace FlightCalculator.Core
{
    public class Airport
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }

        public GeoDMS Latitude { get; set; }

        public GeoDMS Longitude { get; set; }

        public string Label => $"{Code} - {Name}";
    }
}