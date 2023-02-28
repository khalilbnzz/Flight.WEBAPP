using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Common.Models;
using Flight.WEBAPP.Models;
using Flight.WEBAPP.Services.TOOLS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Geolocation;
using System.Globalization;

namespace Flight.WEBAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICachingHelper _caching;
        private readonly IResponseProvider _response;


        public HomeController(ILogger<HomeController> logger, ICachingHelper cachingHelper, IResponseProvider response)
        {
            _logger = logger;
            _caching = cachingHelper;
            _response = response;
        }

        public IActionResult Index()
        {
            
            FlightModel flightModel = new FlightModel();
            if (flightModel.data == null)
            {
                flightModel.data = new List<Datuma>();
            }
            var getList = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd"), null);
            HttpResponseMessage responseAPI = null;
            FlightResponse flightData = null;
            if (getList.Result == null)
            {
                responseAPI = _response.GetListFlightResponse(out string pResponse);
                flightData = JsonConvert.DeserializeObject<FlightResponse>(pResponse);

                flightModel.data = new List<Datuma>();
                foreach (var item in flightData.data.Where(x => x.flight_date.Equals(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))).ToList())
                {
                    Datuma data = new Datuma
                    {
                        aircraft = item.aircraft,
                        airline = item.airline,
                        arrival = item.arrival,
                        departure = item.departure,
                        flight = item.flight,
                        flight_date = item.flight_date,
                        flight_status = item.flight_status,
                        live = item.live,
                        flight_time = item.arrival.estimated.Subtract(item.departure.estimated).ToString()
                    };

                    flightModel.data.Add(data);
                }

                _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd"), flightModel);
            }
            else
            {
                flightModel = getList.Result;
            }

            return View(flightModel);
        }

        [HttpGet] // Set the attribute to Read
        public ActionResult Read(string numVol)
        {

            var flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd"), null).Result;
            if (flightModel.data.Count != 0)
            {
                flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd")+"_"+ numVol, flightModel).Result;
            }
            
            var valueDetails = flightModel.data.Where(x=>x.flight.number.Equals(numVol)).FirstOrDefault();
            flightModel = new FlightModel();
            var respDepart = _response.GetAirPortDetailsResponse(valueDetails.departure.iata, out string pResponseDepart);
            var desDepart = JsonConvert.DeserializeObject<AirportResponse>(pResponseDepart);
            GeoCordinations geoDepart = new GeoCordinations
            {
                Latitude = desDepart.latitude,
                Longiture = desDepart.longitude,
            };
            var respArr = _response.GetAirPortDetailsResponse(valueDetails.arrival.iata, out string pResponseArr);
            var desArri = JsonConvert.DeserializeObject<AirportResponse>(pResponseArr);
           
            GeoCordinations geoArrive = new GeoCordinations
            {
                Latitude = desArri.latitude,
                Longiture = desArri.longitude
            };

            Coordinate origin = new Coordinate { Latitude = Convert.ToDouble(geoDepart.Latitude, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(geoDepart.Longiture, CultureInfo.InvariantCulture) };
            Coordinate destination = new Coordinate { Latitude = Convert.ToDouble(geoArrive.Latitude, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(geoArrive.Longiture, CultureInfo.InvariantCulture) };

            double distance = GeoCalculator.GetDistance(origin, destination, 1) / 1000;
            valueDetails.distance = distance.ToString();
            flightModel.data.Add(valueDetails);
            return View(flightModel);


        }
    }
}