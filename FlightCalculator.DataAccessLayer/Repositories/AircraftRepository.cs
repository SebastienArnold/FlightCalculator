using System.Collections.Generic;
using System.Data;
using System.Linq;
using FlightCalculator.Core;
using FlightCalculator.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.DataAccessLayer.Repositories
{
    public class AircraftRepository : IAircraftRepository
    {
        private readonly ILogger _logger;
        private readonly IDbProvider _dbProvider;

        public AircraftRepository(ILoggerFactory loggerFactory, IDbProvider dbProvider)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _logger.LogDebug("Construct AircraftRepository");

            _dbProvider = dbProvider;
        }

        public Aircraft GetAircraftById(int aircraftId)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM Aircraft WHERE Id = {aircraftId}";
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();

                    var aircraft = new List<Aircraft>();
                    while (reader.Read())
                    {
                        aircraft.Add(new Aircraft
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Name = (string)reader["Name"],
                            ConsumptionPerKilometer = double.Parse(reader["ConsumptionPerKilometer"].ToString()),
                            TakeoffEffort = double.Parse(reader["TakeoffEffort"].ToString())
                        });
                    }

                    return aircraft.FirstOrDefault(a => a.Id == aircraftId);
                }
            }
        }

        public List<Aircraft> GetAllAircraft()
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Aircraft";
                    command.CommandType = CommandType.Text;

                    var reader = command.ExecuteReader();

                    var aircraft = new List<Aircraft>();
                    while (reader.Read())
                    {
                        aircraft.Add(new Aircraft
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Name = (string) reader["Name"],
                            ConsumptionPerKilometer = double.Parse(reader["ConsumptionPerKilometer"].ToString()),
                            TakeoffEffort = double.Parse(reader["TakeoffEffort"].ToString())
                        });
                    }

                    return aircraft;
                }
            }
        }

        public void CreateAircraft(Aircraft aircraft)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Aircraft (Name, ConsumptionPerKilometer, TakeoffEffort) "+
                                          $"VALUES ('{aircraft.Name}', {aircraft.ConsumptionPerKilometer}, {aircraft.TakeoffEffort})";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAircraft(Aircraft aircraft)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Aircraft SET Name = '{aircraft.Name}', ConsumptionPerKilometer = {aircraft.ConsumptionPerKilometer},"+
                                          $" TakeoffEffort = {aircraft.TakeoffEffort} WHERE Id = {aircraft.Id}";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAircraft(int aircraftId)
        {
            using (var connection = _dbProvider.Create())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Aircraft WHERE Id = {aircraftId}";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
