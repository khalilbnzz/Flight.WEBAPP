using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Common.Models;
using Flight.WEBAPP.Services.TOOLS;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Flight.WEBAPP.Common
{
    public class ResponseProvider : IResponseProvider
    {
        private readonly ConfigurationModel _ConfigurationModel;
        private readonly string _urlFlight;
        private readonly string _valueFlight;
        private readonly string _urlAirportLatLong;
        private readonly string _urlAirportCode;
        private readonly string _urlLabs;
        private readonly string _keyAirLabs;


        public ResponseProvider()
        {
            _ConfigurationModel = new ConfigurationModel();
            _urlFlight = _ConfigurationModel.AppConfiguration.flightAPIKEY.link;
            _valueFlight = _ConfigurationModel.AppConfiguration.flightAPIKEY.value;
            _urlAirportLatLong = _ConfigurationModel.AppConfiguration.airportAppSetting.linkLatLong;
            _urlAirportCode = _ConfigurationModel.AppConfiguration.airportAppSetting.linkCode;
            _urlLabs = _ConfigurationModel.AppConfiguration.AirLabs.EndPoint;
            _keyAirLabs = _ConfigurationModel.AppConfiguration.AirLabs.ApiKey;

        }

        public HttpResponseMessage GetAirPortDetailsResponse(string pIataCode,out string pResponse)
        {
            string _completeLink = string.Concat(_urlAirportLatLong, pIataCode);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }

        public HttpResponseMessage GetAirPortDetailsResponse(string pLat, string pLon, out string pResponse)
        {
            string _completeLink = string.Concat(_urlAirportCode, pLat,"/",pLon);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }

        public HttpResponseMessage GetListFlightResponse(out string pResponse)
        {
            string _completeLink = string.Concat(_urlFlight, _valueFlight);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }

        public HttpResponseMessage GetAirPlaneInfo(string pIadaCode, out string pResponse)
        {
            string _completeLink = string.Concat(_urlLabs, "flight?flight_iata=", pIadaCode, "&api_key=", _keyAirLabs);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }


        public HttpResponseMessage GetAirPlaneInfo(out string pResponse)
        {
            string _completeLink = string.Concat(_urlLabs, "flights?api_key=", _keyAirLabs);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }
    }
}
