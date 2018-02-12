using System;
using FlightCalculator.Core;
using FlightCalculator.Core.Interfaces;

namespace FlightCalculator.Business
{
    public class FlightCalculator : IFlightCalculator
    {
        /// <summary>
        /// Compute distance between two points localized by latitude/longitude
        /// </summary>
        /// <param name="sourceLatitude"></param>
        /// <param name="sourceLongitude"></param>
        /// <param name="destinationLatitude"></param>
        /// <param name="destinationLongitude"></param>
        /// <returns>Distance in kilometer</returns>
        public double GetDistance(GeoDMS sourceLatitude, GeoDMS sourceLongitude, GeoDMS destinationLatitude, GeoDMS destinationLongitude)
        {
            var radiansOverDegrees = (Math.PI / 180.0);

            var sLatitudeRadians = sourceLatitude.CoordinateAsDouble * radiansOverDegrees;
            var sLongitudeRadians = sourceLongitude.CoordinateAsDouble * radiansOverDegrees;
            var eLatitudeRadians = destinationLatitude.CoordinateAsDouble * radiansOverDegrees;
            var eLongitudeRadians = destinationLongitude.CoordinateAsDouble * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Using 6371 as the number of kilometer around the earth
            var distanceInKilometer = 6371.0 * 2.0 * Math.Atan2(Math.Sqrt(result), Math.Sqrt(1.0-result));
            return Math.Round(distanceInKilometer, 0);
        }

        public double GetConsumption(double distance, double consumptionPerKilometer, double takeoffEffort)
        {
            return Math.Round(distance * consumptionPerKilometer + takeoffEffort, 0);
        }
    }
}
