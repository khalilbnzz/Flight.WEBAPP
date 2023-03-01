using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Common.Models;
using Flight.WEBAPP.Models;
using Flight.WEBAPP.Services.TOOLS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Geolocation;
using System.Globalization;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace Flight.WEBAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICachingHelper _caching;
        private readonly IResponseProvider _response;


        public HomeController(ILogger<HomeController> logger, ICachingHelper cachingHelper,  IResponseProvider response)
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
            var vol = _caching.GetValue<VolModel>(String.Concat(DateTime.Now.ToString("ddMMyyy"), "DB"));
            if (getList.Result == null)
            {
                responseAPI = _response.GetListFlightResponse(out string pResponse);
                flightData = JsonConvert.DeserializeObject<FlightResponse>(pResponse);
                int i = 0;
                flightModel.data = new List<Datuma>();
                Dictionary<int, Datuma> keyValues = new Dictionary<int, Datuma>();
                foreach (var item in flightData.data.Take(5).Where(x => x.flight_date.Equals(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"))).OrderByDescending(x=>x.flight_date).ToList())
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
                        flight_time = item.arrival.estimated.Subtract(item.departure.estimated).ToString(),
                        Identifier = i
                    };

                    flightModel.data.Add(data);
                    keyValues.Add(i, data);
                    i++;
                }
                
                _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), flightModel);
                _caching.GetSetAsyncDictionnary<Datuma>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), keyValues);
            }
            else
            {
                flightModel = getList.Result;
            }
            if (vol != null)
            {
                var lastIdentifier = flightModel.data.LastOrDefault().Identifier;
                flightModel.data.Add(new Datuma
                {
                    flight_date = vol.HeureDepart.ToString(),
                    flight_status = "NEW ADDED",
                    flight = new Flight.WEBAPP.Common.Models.Flight
                    {
                        number = vol.NumeroVol
                    },
                    Identifier = lastIdentifier + 1
                });
            }
            return View(flightModel);
        }

        [HttpGet] // Set the attribute to Read
        public ActionResult Read(string numVol)
        {

            var flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null).Result;
            var flightModelBeta = _caching.GetSetAsyncDictionnary<Datuma>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null).Result;
            if (flightModel != null && flightModel.data.Count != 0)
            {
                flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")+"_"+ numVol, flightModel).Result;
                var flightOF = flightModel.data.FirstOrDefault(x => x.flight is null);
                if (flightOF != null)
                {
                    var flight = flightModel.data.Find(x => x.flight.number == numVol);
                    if (flight != null)
                    {
                        flightModel = new FlightModel();

                        #region Coordinate GPS
                        var respDepart = _response.GetAirPortDetailsResponse(flight.departure.iata, out string pResponseDepart);
                        var desDepart = JsonConvert.DeserializeObject<AirportResponse>(pResponseDepart);
                        GeoCordinations geoDepart = new GeoCordinations
                        {
                            Latitude = desDepart.latitude,
                            Longiture = desDepart.longitude,
                        };
                        var respArr = _response.GetAirPortDetailsResponse(flight.arrival.iata, out string pResponseArr);
                        var desArri = JsonConvert.DeserializeObject<AirportResponse>(pResponseArr);

                        GeoCordinations geoArrive = new GeoCordinations
                        {
                            Latitude = desArri.latitude,
                            Longiture = desArri.longitude
                        };

                        Coordinate origin = new Coordinate { Latitude = Convert.ToDouble(geoDepart.Latitude, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(geoDepart.Longiture, CultureInfo.InvariantCulture) };
                        Coordinate destination = new Coordinate { Latitude = Convert.ToDouble(geoArrive.Latitude, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(geoArrive.Longiture, CultureInfo.InvariantCulture) };
                        #endregion

                        double distance = GeoCalculator.GetDistance(origin, destination, 1) / 1000;
                        flight.distance = distance.ToString();
                        flightModel.data.Add(flight);
                        return View(flightModel);
                    }
                }
                else
                {
                    var vol = _caching.GetValue<VolModel>(String.Concat(DateTime.Now.ToString("ddMMyyy"), "DB"));
                    if (vol == null)
                    {
                        return View();
                    }
                    flightModel = new FlightModel();
                    flightModel.data.Add(new Datuma
                    {
                        flight_time = vol.HeureArrive.Subtract(vol.HeureDepart).ToString(),
                        distance = "",
                        departure = new Departure
                        {
                            airport = vol.AeroportDepart
                        },
                        arrival = new Arrival
                        {
                            airport = vol.AeroportArrive
                        },
                        airline = new Airline
                        {
                            name = vol.ModelAvion
                        }

                    });
                }

            }
            
            return View(flightModel);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VolModel pModel)
        {
            try
            {
            
                var vol = _caching.GetValue<VolModel>(String.Concat(DateTime.Now.ToString("ddMMyyy"), "DB"));
                if (vol == null)
                {
                    if (ModelState.IsValid)
                    {
                        _caching.SetValue<VolModel>(String.Concat(DateTime.Now.ToString("ddMMyyy"), "DB"), pModel);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Numero de vol existe deja, merci de reessayer";
                    return View(null);
                }
                
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(pModel);
        }
    }
}