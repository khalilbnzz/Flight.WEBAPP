using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Common.Models;
using Flight.WEBAPP.Models;
using Flight.WEBAPP.Services.TOOLS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

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
            var getList = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd"), null);
            HttpResponseMessage responseAPI = null;
            FlightResponse flightData = null;
            if (getList.Result == null)
            {
                responseAPI = _response.GetAPIResponse(out string pResponse);
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
                        live = item.live
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
            FlightModel flightModel = _caching.GetSetAsyncList<FlightModel>(DateTime.Now.ToString("yyyy-MM-dd"), null).Result;
            var valueDetails = flightModel.data.Where(x=>x.flight.number.Equals(numVol)).FirstOrDefault();
            flightModel.data.Clear();
            flightModel.data.Add(valueDetails);

            return View(flightModel);


        }
    }
}