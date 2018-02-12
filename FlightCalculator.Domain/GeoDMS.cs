namespace FlightCalculator.Core
{
    public enum CardinalEnum
    {
        N,
        E,
        S,
        W
    }

    /// <summary>
    /// Geolocalisation expressed in Degree, Miunte, Seconds, Cardinal
    /// </summary>
    public class GeoDMS
    {
        public double Degree { get; set; }
        public double Minute { get; set; }
        public double Second { get; set; }
        public CardinalEnum Cardinal { get; set; }

        public double CoordinateAsDouble => (Degree + Minute / 60 + Second / 3600) * (Cardinal == CardinalEnum.N || Cardinal == CardinalEnum.E ? 1 : -1);
    }
}
