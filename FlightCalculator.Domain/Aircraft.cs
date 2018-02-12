namespace FlightCalculator.Core
{
    public class Aircraft
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Consumption in kg/km.
        /// </summary>
        public double ConsumptionPerKilometer { get; set; }

        /// <summary>
        /// Expressed in kg of neccessary kerosene.
        /// </summary>
        public double TakeoffEffort { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}