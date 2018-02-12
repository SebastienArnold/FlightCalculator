using System.Collections.Generic;
using System.Data;
using System.Linq;
using FlightCalculator.Core;
using FlightCalculator.Core.Interfaces;

namespace FlightCalculator.DataAccessLayer.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly List<Flight> _flights = new List<Flight>();

        private readonly IFlightCalculator _flightCalculator;
        private readonly IDbProvider _dbProvider;
        private readonly IAirportRepository _airportRepository;

        public FlightRepository(IFlightCalculator flightCalculator, IDbProvider dbProvider, IAirportRepository airportRepository)
        {
            _flightCalculator = flightCalculator;
            _dbProvider = dbProvider;
            _airportRepository = airportRepository;
        }

        public Flight GetFlightById(int flightId)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT f.*, a.id AircraftId, a.name Name, a.ConsumptionPerKilometer, a.TakeoffEffort " +
                        " FROM Flight f INNER JOIN aircraft a ON f.AircraftId = a.Id " +
                        $" WHERE f.Id = {flightId}";

                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();

                    var flights = new List<Flight>();
                    while (reader.Read())
                    {
                        var aircraft = new Aircraft
                        {
                            Id = int.Parse(reader["AircraftId"].ToString()),
                            Name = (string)reader["Name"],
                            ConsumptionPerKilometer = double.Parse(reader["ConsumptionPerKilometer"].ToString()),
                            TakeoffEffort = double.Parse(reader["TakeoffEffort"].ToString())
                        };

                        var flight = new Flight
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            FromAirport = _airportRepository.GetAirportByCode((string)reader["FromAirportCode"]),
                            ToAirport = _airportRepository.GetAirportByCode((string)reader["ToAirportCode"]),
                            Aircraft = aircraft,
                            FuelConsumption = double.Parse(reader["FuelConsumption"].ToString()),
                            Distance = double.Parse(reader["Distance"].ToString())
                        };

                        flights.Add(flight);
                    }

                    return flights.FirstOrDefault();
                }
            }
        }
        
        public List<Flight> GetAllFlights()
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT f.*, a.id AircraftId, a.name Name, a.ConsumptionPerKilometer, a.TakeoffEffort "+
                                          " FROM Flight f INNER JOIN aircraft a ON f.AircraftId = a.Id";
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();

                    var flights = new List<Flight>();
                    while (reader.Read())
                    {
                        var aircraft = new Aircraft
                        {
                            Id = int.Parse(reader["AircraftId"].ToString()),
                            Name = (string) reader["Name"],
                            ConsumptionPerKilometer = double.Parse(reader["ConsumptionPerKilometer"].ToString()),
                            TakeoffEffort = double.Parse(reader["TakeoffEffort"].ToString())
                        };

                        var flight = new Flight
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            FromAirport = _airportRepository.GetAirportByCode((string)reader["FromAirportCode"]),
                            ToAirport = _airportRepository.GetAirportByCode((string)reader["ToAirportCode"]),
                            Aircraft = aircraft,
                            FuelConsumption = double.Parse(reader["FuelConsumption"].ToString()),
                            Distance = double.Parse(reader["Distance"].ToString())
                        };

                        flights.Add(flight);
                    }

                    return flights;
                }
            }
        }

        public void CreateFlight(Flight flight)
        {
            flight.Distance = _flightCalculator.GetDistance(flight.FromAirport.Latitude, flight.FromAirport.Longitude,
                flight.ToAirport.Latitude, flight.ToAirport.Longitude);
            flight.FuelConsumption = _flightCalculator.GetConsumption(flight.Distance, flight.Aircraft.ConsumptionPerKilometer,
                    flight.Aircraft.TakeoffEffort);

            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Flight (FromAirportCode, ToAirportCode, AircraftId, FuelConsumption, Distance) "+
                                          $"VALUES ('{flight.FromAirport.Code}', '{flight.ToAirport.Code}', {flight.Aircraft.Id}, "+
                                          $"{flight.FuelConsumption}, {flight.Distance})";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateFlight(Flight flight)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Flight SET FromAirportCode = '{flight.FromAirport.Code}', ToAirportCode = '{flight.ToAirport.Code}',"+
                                          $" AircraftId = {flight.Aircraft.Id}, FuelConsumption = {flight.FuelConsumption}, Distance = {flight.Distance}"+
                                          $" WHERE Id = {flight.Id}";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFlight(int flightId)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Flight WHERE Id = {flightId}";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
