using System.Diagnostics;
using System.Linq;
using FlightCalculator.Core;
using FlightCalculator.Core.Interfaces;
using FlightCalculator.Host.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        private readonly IAircraftRepository _aircraftRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightRepository _flightRepository;

        public HomeController(
            ILoggerFactory loggerFactory,
            IAircraftRepository aircraftRepository,
            IAirportRepository airportRepository,
            IFlightRepository flightRepository)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _logger.LogDebug("Construct HomeController");

            _aircraftRepository = aircraftRepository;
            _airportRepository = airportRepository;
            _flightRepository = flightRepository;
        }

        public IActionResult Report()
        {
            _logger.LogDebug("Report called");

            // todo: remove this test data when using database
            if (_flightRepository.GetAllFlights().Count == 0)
            {
                var fSample = new Flight
                {
                    FromAirport = _airportRepository.GetAirportByCode("LME"),
                    ToAirport = _airportRepository.GetAirportByCode("CDG"),
                    Aircraft = _aircraftRepository.GetAircraftById(1)
                };
                _flightRepository.CreateFlight(fSample);
            }

            var viewModel = new ReportViewModel
            {
                Aircraft = _aircraftRepository.GetAllAircraft(),
                Flights = _flightRepository.GetAllFlights()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
