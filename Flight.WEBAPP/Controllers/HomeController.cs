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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(FlightModel model)
        {

            var responseAPI = _response.GetAPIResponse(out string pResponse);
            var flightData = JsonConvert.DeserializeObject<FlightResponse>(pResponse);
            List<Datum> flightList = new List<Datum>();
            foreach (var item in flightData.data.Where(x=>x.flight_date.Equals(DateTime.Now.ToString("yyyy-MM-dd"))).ToList())
            {
                flightList.Add(item);
            }
            _caching.GetSetAsyncList(DateTime.Now.ToString("yyyy-MM-dd"), flightList);

            var getList = _caching.GetSetAsyncList(DateTime.Now.ToString("yyyy-MM-dd"), flightList);
            if (getList.Result.Count() != 0)
            {

            }


            string message = "Created the record successfully";

            // To display the message on the screen
            // after the record is created successfully
            ViewBag.Message = message;

            // write @Viewbag.Message in the created
            // view at the place where you want to
            // display the message
            return View();
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