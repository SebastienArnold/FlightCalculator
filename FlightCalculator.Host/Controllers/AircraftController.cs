using System.Diagnostics;
using FlightCalculator.Core.Interfaces;
using FlightCalculator.Host.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.Host.Controllers
{
    public class AirCraftController : Controller
    {
        private readonly ILogger _logger;

        private readonly IAircraftRepository _aircraftRepository;

        public AirCraftController(ILoggerFactory loggerFactory,
            IAircraftRepository aircraftRepository)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _logger.LogDebug("Construct AirCraftController");

            _aircraftRepository = aircraftRepository;
        }

        public IActionResult List(int? selectId)
        {
            _logger.LogDebug(string.Concat("List aircraft", selectId.HasValue ? $" (, {selectId} is selected" : string.Empty));

            var viewModel = new AircraftViewModel {ExistingAircraft = _aircraftRepository.GetAllAircraft()};

            if (selectId.HasValue)
            {
                var editedAircraft = _aircraftRepository.GetAircraftById(selectId.Value);
                viewModel.SelectedAircraft = editedAircraft;
            }

            return View("Aircraft", viewModel);
        }

        public IActionResult Submit(AircraftViewModel parameter)
        {
            _logger.LogDebug("Submiting aircraft");

            if (_aircraftRepository.GetAircraftById(parameter.SelectedAircraft.Id) != null)
            {
                _logger.LogDebug("Aircraft Id already exist");
                _aircraftRepository.UpdateAircraft(parameter.SelectedAircraft);
            }
            else
            {
                _aircraftRepository.CreateAircraft(parameter.SelectedAircraft);
            }

            return new RedirectToActionResult("List", "Aircraft", null);
        }

        public IActionResult Delete(int? selectId)
        {
            _logger.LogDebug($"Deleting flight {selectId}");
            if (selectId.HasValue)
            {
                _aircraftRepository.DeleteAircraft(selectId.Value);
            }
            else
            {
                _logger.LogWarning("Aircraft to delete doesn't exist");
            }

            return new RedirectToActionResult("List", "Aircraft", null);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
