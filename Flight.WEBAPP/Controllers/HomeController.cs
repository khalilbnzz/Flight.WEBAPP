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
using NoDb;
using System.Net;

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
            var RespoAllFlight = _response.GetAirPlaneInfo(out string pResponseInfo);

            FlightModel flightModel = new FlightModel();
            if (flightModel.data == null)
            {
                flightModel.data = new List<Datuma>();
            }
            var getList = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null);
            HttpResponseMessage responseAPI = null;
            FlightResponse flightData = null;
            
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
                
            }
            else
            {
                flightModel = new FlightModel();
                flightModel = getList.Result;
            }
            var vol = _caching.GetSetAsyncList<VolModel>(DateTime.Now.ToString("yyyy-MM-dd"), null);
            if (vol.Result != null)
            {
                flightModel.dataVol = new List<VolModel> { vol.Result };
            }
            
            _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), flightModel);

            return View(flightModel);
        }

        [HttpGet] // Set the attribute to Read
        public ActionResult Read(string numVol, bool created)
        {
            var flightModel = new FlightModel();
            if (!created)
            {
                flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null).Result;

                if (flightModel != null && flightModel.data.Count != 0)
                {
                    var flight = flightModel.data.Find(x => x.flight.number == numVol);
                    var responseAPI = _response.GetAirPlaneInfo(flight.flight.iata, out string pResponse);
                    

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
                    flightModel = new FlightModel();
                    double distance = GeoCalculator.GetDistance(origin, destination, 1) / 1000;
                    flight.distance = distance.ToString();
                    flightModel.data.Add(flight);
                }

            }
            else
            {
                var flightOF = flightModel.data.FirstOrDefault(x => x.flight is null);
                if (flightOF == null)
                {
                    var vol = _caching.GetValue<VolModel>(String.Concat(DateTime.Now.ToString("ddMMyyy"), null));
                    if (vol == null)
                    {
                        return View();
                    }
                    if (vol != null)
                    {
                        flightModel.dataVol = new List<VolModel> { vol };
                    }
                }

            }
            return View(flightModel);

        }


        [HttpGet] // Set the attribute to Read
        public ActionResult ReadCreated(string numVol, bool created)
        {
            var flightModel = new FlightModel();
            if (!created)
            {
                flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null).Result;

                if (flightModel != null && flightModel.data.Count != 0)
                {
                    var flight = flightModel.data.Find(x => x.flight.number == numVol);
                    var responseAPI = _response.GetAirPlaneInfo(flight.flight.iata, out string pResponse);


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
                    flightModel = new FlightModel();
                    double distance = GeoCalculator.GetDistance(origin, destination, 1) / 1000;
                    flight.distance = distance.ToString();
                    flightModel.data.Add(flight);
                }

            }
            else
            {
                flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null).Result;
                
                if (flightModel.dataVol.Count != 0)
                {
                    var vol = flightModel.dataVol.FirstOrDefault(x=>x.NumeroVol.Equals(numVol));
                    if (vol == null)
                    {
                        return View();
                    }
                    if (vol != null)
                    {
                        flightModel.dataVol = new List<VolModel> { vol };
                    }
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

                var vol = _caching.GetSetAsyncList<VolModel>(DateTime.Now.ToString("yyyy-MM-dd"), null);
                
                if (vol.Result == null)
                {
                    if (ModelState.IsValid)
                    {
                        _caching.GetSetAsyncList<VolModel>(DateTime.Now.ToString("yyyy-MM-dd"), pModel);
                        var flightModel = new FlightModel
                        {
                            dataVol = new List<VolModel> { pModel }
                        };
                        _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), flightModel);
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

        [HttpGet]
        public ActionResult Edit(string numVol)
        {
            if (numVol == null)
            {
                return View(null);
            }
            var vol = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), null);
            var volF = vol.Result.dataVol.FirstOrDefault(x => x.NumeroVol.Equals(numVol));
            VolModel pModel = new VolModel
            {
                NumeroVol = numVol,
                HeureArrive = volF.HeureArrive,
                HeureDepart = volF.HeureDepart,
                AeroportArrive = volF.AeroportArrive,
                AeroportDepart = volF.AeroportDepart,
                ModelAvion = volF.ModelAvion,
                VilleArrive = volF.VilleArrive,
                VilleDepart = volF.VilleDepart
            };

            return View(pModel);
        }
    }
}