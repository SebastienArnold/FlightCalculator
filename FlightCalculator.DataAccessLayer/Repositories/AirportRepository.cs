using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using FlightCalculator.Core;
using FlightCalculator.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.DataAccessLayer.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ILogger _logger;

        private const string CountryCacheKey = "Airports-Countries";
        private const string AirportsCacheKey = "Airports-Country:{0}";
        private const string AirportCacheKey = "Airports-Id:{0}";

        private readonly string _hostUrl;
        private readonly int _cacheRetentionMinute;
        private readonly IMemoryCache _memoryCache;
        private readonly string _proxyUrl;
        private readonly string _proxyUser;
        private readonly string _proxyPassword;

        public AirportRepository(ILoggerFactory loggerFactory, IMemoryCache memoryCache, string hostUrl, int cacheRetentionMinute)
            :this(loggerFactory, memoryCache, hostUrl, cacheRetentionMinute, string.Empty, string.Empty, string.Empty)
        {}

        public AirportRepository(ILoggerFactory loggerFactory, IMemoryCache memoryCache, string hostUrl, int cacheRetentionMinute,
            string proxyUrl, string proxyUser, string proxyPassword)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _logger.LogDebug("Construct AirportRepository");

            _hostUrl = hostUrl;
            _cacheRetentionMinute = cacheRetentionMinute;
            _memoryCache = memoryCache;
            _proxyUrl = proxyUrl;
            _proxyUser = proxyUser;
            _proxyPassword = proxyPassword;
        }

        private List<string> ExtractCountryFromResponse(string content)
        {
            _logger.LogDebug("Extracting countries from service response");
            var countries = new List<string>();
            if (string.IsNullOrEmpty(content))
            {
                return countries;
            }

            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(content);
                
                foreach (XmlNode node in xml.ChildNodes[1].ChildNodes[0])
                {
                    countries.Add(node.ChildNodes[0].InnerText);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading countries response");
            }

            _logger.LogDebug($"{countries.Count} countries found");
            return countries;
        }

        private List<Airport> ExtractAirportFromResponse(string content)
        {
            _logger.LogDebug("Extracting airport from service response");
            var airports = new List<Airport>();
            if (string.IsNullOrEmpty(content))
            {
                return airports;
            }

            try
            {
                var xml = new XmlDocument();
                xml.LoadXml(content);

                foreach (XmlElement node in xml.ChildNodes[1].ChildNodes[0])
                {
                    var code = node.GetElementsByTagName("AirportCode")?[0]?.InnerText;
                    if (airports.Any(a => a.Code == code))
                    {
                        continue;
                    }

                    var airport = new Airport
                    {
                        Code = node.GetElementsByTagName("AirportCode")?[0]?.InnerText,
                        Country = node.GetElementsByTagName("Country")?[0]?.InnerText,
                        Name = node.GetElementsByTagName("CityOrAirportName")?[0]?.InnerText,
                        Latitude = new GeoDMS
                        {
                            Degree = double.Parse(node.GetElementsByTagName("LatitudeDegree")?[0]?.InnerText),
                            Minute = double.Parse(node.GetElementsByTagName("LatitudeMinute")?[0]?.InnerText),
                            Second = double.Parse(node.GetElementsByTagName("LatitudeSecond")?[0]?.InnerText),
                            Cardinal = Enum.Parse<CardinalEnum>(node.GetElementsByTagName("LatitudeNpeerS")?[0]?.InnerText)
                        },
                        Longitude = new GeoDMS
                        {
                            Degree = double.Parse(node.GetElementsByTagName("LongitudeDegree")?[0]?.InnerText),
                            Minute = double.Parse(node.GetElementsByTagName("LongitudeMinute")?[0]?.InnerText),
                            Second = double.Parse(node.GetElementsByTagName("LongitudeSeconds")?[0]?.InnerText),
                            Cardinal = Enum.Parse<CardinalEnum>(node.GetElementsByTagName("LongitudeEperW")?[0]?.InnerText)
                        }
                    };
                    airports.Add(airport);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading airports response");
            }

            _logger.LogDebug($"{airports.Count} countries found");
            return airports;
        }

        private string GetContentFromWebservice(string request)
        {
            _logger.LogDebug($"Requesting airport data to webservicex. Request:{request}");

            try
            {
                var url = $"{_hostUrl}/{request}";
                var httpClientHandler = new HttpClientHandler();

                if (!string.IsNullOrEmpty(_proxyUrl))
                {
                    _logger.LogDebug($"Proxy detected: {_proxyUrl}");
                    var proxy = new WebProxy(_proxyUrl)
                    {
                        UseDefaultCredentials = true
                    };

                    if (!string.IsNullOrEmpty(_proxyUser))
                    {
                        proxy.Credentials = new NetworkCredential(_proxyUser, _proxyPassword);
                    }

                    httpClientHandler.Proxy = proxy;
                    httpClientHandler.UseProxy = true;
                }
                
                var httpClient = new HttpClient(httpClientHandler);

                var countryResponse = Task.Run(() => httpClient.GetAsync(url)).Result;
                return Task.Run(() => countryResponse.Content.ReadAsStringAsync()).Result
                    .Replace("&lt;", "<").Replace("&gt;", ">");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing request to webservicex. Request:{request}");
                return string.Empty;
            }
        }

        public List<string> GetAllCountries()
        {
            List<string> cacheEntry;

            // Look for cache key.
            if (!_memoryCache.TryGetValue(CountryCacheKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                var content = GetContentFromWebservice("country.asmx/GetCountries");
                cacheEntry = ExtractCountryFromResponse(content);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheRetentionMinute));

                // Save data in cache.
                _memoryCache.Set(CountryCacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        public List<Airport> GetAllAirport(string country)
        {
            List<Airport> cacheEntry;
            var cacheKey = string.Format(AirportsCacheKey, country);

            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                var airports = GetContentFromWebservice($"airport.asmx/GetAirportInformationByCountry?country={country}");
                cacheEntry = ExtractAirportFromResponse(airports);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheRetentionMinute));

                // Save data in cache.
                _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        public Airport GetAirportByCode(string airportCode)
        {
            Airport cacheEntry;
            var cacheKey = string.Format(AirportCacheKey, airportCode);

            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out cacheEntry))
            {
                // Key not in cache, so get data.
                var airports = GetContentFromWebservice($"airport.asmx/getAirportInformationByAirportCode?airportCode={airportCode}");
                cacheEntry = ExtractAirportFromResponse(airports)[0];

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheRetentionMinute));

                // Save data in cache.
                _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}