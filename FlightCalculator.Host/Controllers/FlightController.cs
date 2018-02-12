using System.Diagnostics;
using FlightCalculator.Core.Interfaces;
using FlightCalculator.Host.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.Host.Controllers
{
    public class FlightController : Controller
    {
        private readonly ILogger _logger;

        private readonly IFlightRepository _flightRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IAirportRepository _airportRepository;

        public FlightController
            (ILoggerFactory loggerFactory,
            IFlightRepository flightRepository,
            IAircraftRepository aircraftRepository,
            IAirportRepository airportRepository
            )
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _logger.LogDebug("Construct FlightController");

            _flightRepository = flightRepository;
            _aircraftRepository = aircraftRepository;
            _airportRepository = airportRepository;
        }

        public JsonResult GetAirports(string country)
        {
            _logger.LogDebug($"Retrieve airports for country '{country}'");

            var airports = _airportRepository.GetAllAirport(country);
            return Json(new SelectList(airports, "Code", "Label"));
        }

        public IActionResult List(int? selectId)
        {
            _logger.LogDebug(string.Concat("List flights", selectId.HasValue ? $" (, {selectId} is selected" : string.Empty));

            var viewModel = new FlightViewModel
            {
                ExistingFlights = _flightRepository.GetAllFlights(),
                ExistingAircraft = _aircraftRepository.GetAllAircraft(),
                Countries = _airportRepository.GetAllCountries()
            };

            if (selectId.HasValue)
            {
                var editedFlight = _flightRepository.GetFlightById(selectId.Value);
                viewModel.SelectedFlight = editedFlight;
                viewModel.FromCountry = viewModel.SelectedFlight.FromAirport.Country;
                viewModel.ToCountry = viewModel.SelectedFlight.ToAirport.Country;
            }

            return View("Flight", viewModel);
        }

        public IActionResult Submit(FlightViewModel parameter)
        {
            _logger.LogDebug("Submiting flight");

            if (parameter?.SelectedFlight?.Aircraft?.Id == null)
            {
                _logger.LogWarning("Submit flight cancelled: aircraft id is null");
                return new RedirectToActionResult("List", "Flight", null);
            }
            if (parameter?.SelectedFlight?.FromAirport?.Code == null)
            {
                _logger.LogWarning("Submit flight cancelled: departure airport code is null");
                return new RedirectToActionResult("List", "Flight", null);
            }
            if (parameter?.SelectedFlight?.ToAirport?.Code == null)
            {
                _logger.LogWarning("Submit flight cancelled: arrival airport code is null");
                return new RedirectToActionResult("List", "Flight", null);
            }

            parameter.SelectedFlight.Aircraft =
                _aircraftRepository.GetAircraftById(parameter.SelectedFlight.Aircraft.Id);
            parameter.SelectedFlight.FromAirport =
                _airportRepository.GetAirportByCode(parameter.SelectedFlight.FromAirport.Code);
            parameter.SelectedFlight.ToAirport =
                _airportRepository.GetAirportByCode(parameter.SelectedFlight.ToAirport.Code);
            
            if (_flightRepository.GetFlightById(parameter.SelectedFlight.Id) != null)
            {
                _logger.LogDebug("Flight Id already exist");
                _flightRepository.UpdateFlight(parameter.SelectedFlight);
            }
            else
            {
                _flightRepository.CreateFlight(parameter.SelectedFlight);
            }

            return new RedirectToActionResult("List", "Flight", null);
        }

        public IActionResult Delete(int? selectId)
        {
            _logger.LogDebug($"Deleting flight {selectId}");
            if (selectId.HasValue)
            {
                _flightRepository.DeleteFlight(selectId.Value);
            }
            else
            {
                _logger.LogWarning("Flight to delete doesn't exist");
            }

            return new RedirectToActionResult("List", "Flight", null);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
