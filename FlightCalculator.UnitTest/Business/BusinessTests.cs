using System;
using FlightCalculator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightCalculator.UnitTest.Business
{
    [TestClass]
    public class BusinessTests
    {
        [TestMethod]
        public void GetConsumptionMustComputeConsumption()
        {
            // Arrange
            FlightCalculator.Business.FlightCalculator flightCalculator = new FlightCalculator.Business.FlightCalculator();
            var distance = 100;
            var consumption = 50;
            var takeoffEffort = 20;

            // Act
            var result = flightCalculator.GetConsumption(distance, consumption, takeoffEffort);

            // Assert
            Assert.AreEqual(5020, result);
        }

        [TestMethod]
        public void GetConsumptionMustRoundResult()
        {
            // Arrange
            FlightCalculator.Business.FlightCalculator flightCalculator = new FlightCalculator.Business.FlightCalculator();
            var distance = 100.25;
            var consumption = 50.2;
            var takeoffEffort = 20.1;

            // Act
            var result = flightCalculator.GetConsumption(distance, consumption, takeoffEffort);

            // Assert
            Assert.AreEqual(5053, result);
        }

        /// <summary>
        /// Take coordonate as XX XX XX C and create GeoDMS
        /// </summary>
        /// <param name="coordonate"></param>
        /// <returns></returns>
        private GeoDMS GeoCoordonateHelper(string coordonate)
        {
            var values = coordonate.Split(" ");

            var geo = new GeoDMS
            {
                Degree = double.Parse(values[0]),
                Minute = double.Parse(values[1]),
                Second = double.Parse(values[2]),
                Cardinal = Enum.Parse<CardinalEnum>(values[3])
            };

            return geo;
        }

        [TestMethod]
        [DataRow("50 03 59 N", "005 42 53 W", "58 38 38 N", "003 04 12 W", 969)]
        [DataRow("50 03 59 S", "005 42 53 W", "58 38 38 N", "003 04 12 W", 12090)]
        [DataRow("50 03 59 S", "005 42 53 E", "58 38 38 N", "003 04 12 W", 12114)]
        [DataRow("50 03 59 S", "005 42 53 E", "58 38 38 S", "003 04 12 W", 1109)]
        [DataRow("50 03 59 S", "005 42 53 E", "58 38 38 S", "003 04 12 E", 969)]
        public void ValidateGeoCoordonateHelper(string fromLat, string fromLong, string toLat, string toLong,
            double expected)
        {
            // Arrange
            FlightCalculator.Business.FlightCalculator flightCalculator = new FlightCalculator.Business.FlightCalculator();

            var fromLatitude = GeoCoordonateHelper(fromLat);
            var fromLongitude = GeoCoordonateHelper(fromLong);
            var toLatitude = GeoCoordonateHelper(toLat);
            var toLongitude = GeoCoordonateHelper(toLong);

            // Act
            var result = flightCalculator.GetDistance(fromLatitude, fromLongitude, toLatitude, toLongitude);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
